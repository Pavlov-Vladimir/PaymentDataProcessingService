using System.Collections;

namespace PDPS.Core.Contracts
{
    public interface IParserFactory<T, Tstatus>
        where T : class
        where Tstatus : class
    {
        IParser<T, Tstatus> Create(string path);
    }
}
