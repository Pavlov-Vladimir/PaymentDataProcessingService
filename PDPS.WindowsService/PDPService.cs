using Microsoft.Extensions.Configuration;
using PDPS.Core;
using PDPS.Core.Converters;
using PDPS.Core.DTOs;
using PDPS.Core.Models;
using PDPS.Core.Parsers;
using PDPS.Core.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace PaymentDataProcessingService.WindowsService
{
    public partial class PDPService : ServiceBase
    {
        private readonly FileSystemWatcher _fileWatcher;
        private readonly TimeWatcher _timeWatcher;
        private readonly ReportWriter _reportWriter;
        private readonly MetaWriter _metaWriter;
        private readonly ServiceConfiguration _configuration;
        private readonly ParserWorker<List<Transaction>, ParserResultStatus> _parserWorker;
        private readonly DataConverter _converter;

        public bool IsFailed { get; private set; } = false;
        public int OutputFileCounter { get; set; } = 0;
        public string OutputFileName { get => $"output{OutputFileCounter + 1}.json"; }

        public PDPService()
        {
            InitializeComponent();

            _parserWorker = new ParserWorker<List<Transaction>, ParserResultStatus>(new ParserFactory());
            _parserWorker.OnCompleted += ParserWorker_OnCompleted;

            _converter = new DataConverter();

            _timeWatcher = new TimeWatcher();
            _timeWatcher.OnTime += TimeWatcher_OnTime;

            _configuration = new ServiceConfiguration();
            _configuration.Manager.AddJsonFile("config.json");
            _configuration.SetConfiguration();

            _fileWatcher = new FileSystemWatcher(_configuration.InputFolder);
            _fileWatcher.Created += FileWatcher_Created;

            _reportWriter = new ReportWriter(_configuration.OutputFolder);
            _metaWriter = new MetaWriter(_configuration.OutputFolder);

        }

        protected override void OnStart(string[] args)
        {
            IsFailed = false;
            _fileWatcher.EnableRaisingEvents = true;
            _timeWatcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            if (!IsFailed)
            {
                int attempt = 100;
                bool isActive = _parserWorker.IsActive && _converter.IsActive && _reportWriter.IsActive;
                while (isActive && attempt-- > 0)  // wait a while if the service is busy
                {
                    Task.Delay(50);
                }
                ClearInputFolder();
            }
            _fileWatcher.EnableRaisingEvents = false;
            _timeWatcher.EnableRaisingEvents = false;
        }

        private async void TimeWatcher_OnTime(object obj)
        {
            var metaReport = new MetaReport()
            {
                ParsedFiles = _parserWorker.ParsedFiles,
                ParsedLines = _parserWorker.ParsedLines,
                Errors = _parserWorker.ErrorLines,
                InvalidFiles = _parserWorker.InvalidFiles
            };
            await _metaWriter.Write("meta.log", metaReport);
        }

        private async void ParserWorker_OnCompleted(object sender, List<Transaction> data)
        {
            if (data == null || data.Count == 0)
                return;

            try
            {
                var report = await _converter.Convert(data);

                if (report != null && report.Cities.Count != 0)
                {
                    await _reportWriter.Write(OutputFileName, report);
                    OutputFileCounter++;
                }
            }
            catch (Exception)
            {
                IsFailed = true;
                OnStop();
            }
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            _parserWorker.Start(e.FullPath);
        }

        private void ClearInputFolder()
        {
            var directory = new DirectoryInfo(_configuration.InputFolder);
            if (directory.Exists)
            {
                FileInfo[] files = directory.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }
        }

        internal void ConsoleRun()
        {
            while (true)
            {
                Console.WriteLine("Press any key to start PDPService...");
                Console.ReadKey();
                OnStart(null);
                Console.Write(Environment.NewLine);
                Console.WriteLine("Press any key to stop service.");
                Console.ReadKey();
                OnStop();
                Console.Write(Environment.NewLine);
            }
        }
    }
}
