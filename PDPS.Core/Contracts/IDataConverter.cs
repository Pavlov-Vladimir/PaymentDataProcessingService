using System.Collections;
using System.Threading.Tasks;

namespace PDPS.Core.Contracts
{
    public interface IDataConverter<TIn, TOut> where TIn : IEnumerable where TOut : class
    {
        Task<TOut> Convert(TIn data);
    }
}
