using System.Collections;

namespace PDPS.Core.Contracts
{
    public interface IParserFactory<T> where T : IEnumerable
    {
        IParser<T> Create(string path);
    }
}
