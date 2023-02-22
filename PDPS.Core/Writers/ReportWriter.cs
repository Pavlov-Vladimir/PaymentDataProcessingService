using PDPS.Core.Contracts;
using PDPS.Core.Models;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PDPS.Core.Writers
{
    public class ReportWriter : IWriter<Report>
    {
        private readonly string _path;

        public ReportWriter(string path)
        {
            _path = path;
        }

        public async Task Write(string file, Report report)
        {
            string fullPath = Path.Combine(_path, file);
            using (FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync<Report>(fs, report);
            }
        }
    }
}
