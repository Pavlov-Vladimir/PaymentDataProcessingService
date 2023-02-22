using System.Collections;
using System.Threading.Tasks;

namespace PDPS.Core.Contracts
{
    public interface IParser<T> where T : IEnumerable
    {
        int Errors { get; set; }
        int ParsedLines { get; set; }
        string Path { get; set; }

        Task<T> ParseAsync();
    }
}
