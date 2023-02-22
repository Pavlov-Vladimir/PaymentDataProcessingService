﻿using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace PDPS.Core.Parsers
{
    public class ParserFactory : IParserFactory<List<Transaction>>
    {
        public IParser<List<Transaction>> Create(string path)
        {
            string extention = path.Split('.').Last().ToLower();

            return extention switch
            {
                "txt" => new ParserTXT(path),
                "csv" => new ParserCSV(path),
                _ => null
            };
        }
    }
}