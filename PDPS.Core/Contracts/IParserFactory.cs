using System.Collections;

namespace PDPS.Core.Contracts
{
    public interface IParserFactory<T, Tstatus> where T : IEnumerable
    {
        IParser<T, Tstatus> Create(string path);
    }
}
