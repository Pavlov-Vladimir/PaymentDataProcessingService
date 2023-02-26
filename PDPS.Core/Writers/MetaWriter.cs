using PDPS.Core.Contracts;
using PDPS.Core.Models;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace PDPS.Core.Writers
{
    public class MetaWriter : IWriter<MetaReport>
    {
        public string BasePath;
        public string DirectoryPath => $"{BasePath}\\{DateTime.Now:MM-dd-yyyy}";

        public MetaWriter() {  }

        public MetaWriter(string path)
        {
            BasePath = path;
        }

        public async Task Write(string file, MetaReport meta)
        {
            try
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                string fullPath = Path.Combine(DirectoryPath, file);
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                };
                using FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
                await JsonSerializer.SerializeAsync<MetaReport>(fs, meta, options);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
