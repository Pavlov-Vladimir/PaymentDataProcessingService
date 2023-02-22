using System.Collections;
using System.Threading.Tasks;

namespace PDPS.Core.Contracts
{
    public interface IParser<T, Tstatus> where T : IEnumerable
    {
        Tstatus Status { get; set; }

        Task<(T, Tstatus)> ParseAsync();
    }
}
