using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PDPS.Core.Parsers
{
    public class ParserFactory : IParserFactory<List<Transaction>, ParserResultStatus>
    {
        public ParserFactory() { }

        public IParser<List<Transaction>, ParserResultStatus> Create(string path)
        {
            string extention = Path.GetExtension(path);

            return extention switch
            {
                ".txt" => new ParserTXT(path),
                ".csv" => new ParserCSV(path),
                _ => null
            };
        }
    }
}
