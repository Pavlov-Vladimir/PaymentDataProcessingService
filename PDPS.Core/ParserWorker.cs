using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PDPS.Core
{
    public class ParserWorker<T, S> where T : IEnumerable where S : ParserResultStatus
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

        private async void Work(string path)
        {
            var parser = _parserFactory.Create(path);

            var (transactions, status) = await parser.ParseAsync();
            ParsedLines += status.ParsedLines;
            ParsedFiles++;
            if (status.Errors != 0)
            {
                ErrorLines += status.Errors;
                InvalidFiles.Add(path);
            }

            OnCompleted?.Invoke(this, transactions);
            IsActive = false;
        }
    }
}
