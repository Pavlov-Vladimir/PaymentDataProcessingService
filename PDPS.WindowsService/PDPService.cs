using Microsoft.Extensions.Configuration;
using PDPS.Core;
using PDPS.Core.Converters;
using PDPS.Core.DTOs;
using PDPS.Core.Models;
using PDPS.Core.Parsers;
using PDPS.Core.Writers;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        public bool IsFailed { get; private set; }
        public int OutputFileCounter { get; set; }
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
            _fileWatcher = new FileSystemWatcher();
            _reportWriter = new ReportWriter();
            _metaWriter = new MetaWriter();
        }

        protected override void OnStart(string[] args)
        {
            var assemlyFile = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(assemlyFile);
            var configPath = Path.Combine(directory, "config.json");
            if (File.Exists(configPath))
            {
                _configuration.Manager.AddJsonFile(configPath);
                _configuration.SetConfiguration();
            }
            bool isConfigValid = !(string.IsNullOrEmpty(_configuration.InputFolder) &&
                                   string.IsNullOrEmpty(_configuration.OutputFolder));
            if (isConfigValid)
            {
                _fileWatcher.Path = _configuration.InputFolder;
                _fileWatcher.Created += FileWatcher_Created;

                _reportWriter.BasePath = _configuration.OutputFolder;
                _metaWriter.BasePath = _configuration.OutputFolder;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Warning()
                    .WriteTo.File(Path.Combine(_configuration.OutputFolder, "PDPService.log"))
                    .CreateLogger();

                IsFailed = false;
                SetOutputFileCounter();
                _fileWatcher.EnableRaisingEvents = true;
                _timeWatcher.EnableRaisingEvents = true;
            }
            else
            {
                OnStop();
            }
        }

        protected override void OnStop()
        {
            try
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
            catch (Exception ex)
            {
                IsFailed = true;
                if (Log.Logger != null)
                Log.Error(ex.Message);
                Log.CloseAndFlush();
            }
            finally
            {
                Log.CloseAndFlush(); 
            }
        }

        private async void TimeWatcher_OnTime(object obj)
        {
            try
            {
                var metaReport = new MetaReport()
                {
                    ParsedFiles = _parserWorker.ParsedFiles,
                    ParsedLines = _parserWorker.ParsedLines,
                    Errors = _parserWorker.ErrorLines,
                    InvalidFiles = _parserWorker.InvalidFiles
                };
                await _metaWriter.Write("meta.log", metaReport);
                SetOutputFileCounter();
            }
            catch (Exception ex)
            {
                IsFailed = true;
                Log.Error(ex.Message); 
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private void SetOutputFileCounter()
        {
            var path = Path.Combine(_configuration.OutputFolder, DateTime.Now.ToString("MM-dd-yyyy"));
            var directory = new DirectoryInfo(path);

            OutputFileCounter = directory.Exists ? directory.GetFiles().Length : 0;
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
            catch (Exception ex)
            {
                IsFailed = true;
                Log.Error(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                _parserWorker.Start(e.FullPath);
            }
            catch (Exception ex)
            {
                IsFailed = true;
                Log.Error(ex.Message); 
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private void ClearInputFolder()
        {
            try
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
            catch (Exception ex)
            {
                IsFailed = true;
                Log.Error(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
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
