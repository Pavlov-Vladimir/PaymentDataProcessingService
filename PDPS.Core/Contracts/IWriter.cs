using System.Threading.Tasks;

namespace PDPS.Core.Contracts
{
    public interface IWriter<T> where T : class
    {
        Task Write(string fileName, T value);
    }
}
