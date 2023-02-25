using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDPS.Core
{
    public class ParserWorker<T, S> where T : class where S : ParserResultStatus
    {
        private readonly IParserFactory<T, S> _parserFactory;

        public bool IsActive { get; private set; }
        public int ErrorLines { get; set; }
        public int ParsedLines { get; set; }
        public int ParsedFiles { get; set; }
        public List<string> InvalidFiles { get; set; }

        public event Action<object, T> OnCompleted;

        public ParserWorker(IParserFactory<T, S> factory)
        {
            _parserFactory = factory;
            InvalidFiles = new List<string>();
        }

        public void Start(string path)
        {
            IsActive = true;
            Work(path);
        }

        public void Stop()
        {
            IsActive = false;
        }

        public void Reset()
        {
            IsActive = false;
            ErrorLines = 0;
            ParsedLines = 0;
            ParsedFiles = 0;
            InvalidFiles = new List<string>();
        }

        private async void Work(string path)
        {
            var parser = _parserFactory.Create(path);
            var fileName = path.Split('\\').Last();
            if (parser == null)
            {
                InvalidFiles.Add(fileName);
                OnCompleted?.Invoke(this, null);
            }
            else
            {
                await parser.ReadFileAsync();
                var (transactions, status) = await parser.ParseAsync();
                ParsedLines += status.ParsedLines;
                if (status.Errors != 0)
                {
                    ErrorLines += status.Errors;
                    InvalidFiles.Add(fileName);
                }
                OnCompleted?.Invoke(this, transactions);
            }
            ParsedFiles++;
            IsActive = false;
        }
    }
}
