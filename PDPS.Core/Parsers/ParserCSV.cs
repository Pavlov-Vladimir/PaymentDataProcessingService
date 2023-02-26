namespace PDPS.Core.Parsers
{
    public class ParserCSV : ParserBase
    {
        public ParserCSV() { }

        public ParserCSV(string path) : base(path)
        {
        }

        protected override void PrepareContentDependsByParserType()
        {
            FileContent.RemoveAt(0);
            Status.ParsedLines++;
        }
    }
}
