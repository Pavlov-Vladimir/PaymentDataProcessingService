﻿using PDPS.Core.Contracts;
using PDPS.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace PDPS.Core.Writers
{
    public class ReportWriter : IWriter<Report>
    {
        private readonly string _basePath;
        public bool IsActive { get; private set; }

        public string DirectoryPath => $"{_basePath}\\{DateTime.Now:MM-dd-yyyy}";

        public ReportWriter(string path)
        {
            _basePath = path;
        }

        public async Task Write(string file, Report report)
        {
            try
            {
                IsActive = true;
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
                await JsonSerializer.SerializeAsync<List<City>>(fs, report.Cities, options);
            }
            catch (Exception)
            {
                throw;
            }
            finally 
            {
                IsActive = false; 
            }
        }
    }
}
