using System.Collections;
using System.Threading.Tasks;

namespace PDPS.Core.Contracts
{
    public interface IParser<Tout, Tstatus>
        where Tout : class
        where Tstatus : class
    {
        Tstatus Status { get; set; }

        Task ReadFileAsync();
        Task<(Tout, Tstatus)> ParseAsync();
    }
}
