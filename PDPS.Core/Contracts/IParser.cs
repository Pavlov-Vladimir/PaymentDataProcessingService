using System.Collections;
using System.Threading.Tasks;

namespace PDPS.Core.Contracts
{
    public interface IParser<Tout, Tstatus> where Tout : IEnumerable where Tstatus : class
    {
        Tstatus Status { get; set; }

        Task<(Tout, Tstatus)> ParseAsync();
    }
}
