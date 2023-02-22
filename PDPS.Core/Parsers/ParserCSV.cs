using System.IO;

namespace PDPS.Core.Parsers
{
    public class ParserCSV : ParserBase
    {
        public ParserCSV() { }

        public ParserCSV(string path) : base(path)
        {
        }

        protected override void PreprocessStreamDependsFromParserType(StreamReader reader)
        {
            reader.ReadLine();
            ParsedLines++;
        }
    }
}
