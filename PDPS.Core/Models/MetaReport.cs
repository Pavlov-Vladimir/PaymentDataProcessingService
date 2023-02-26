using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PDPS.Core.Models
{
    [Serializable]
    public class MetaReport
    {
        [JsonPropertyName("parsed_files")]
        public int ParsedFiles { get; set; }

        [JsonPropertyName("parsed_lines")]
        public int ParsedLines { get; set; }

        [JsonPropertyName("found_errors")]
        public int Errors { get; set; }

        [JsonPropertyName("invalid_files")]
        public List<string> InvalidFiles { get; set; } = new List<string>();

        public MetaReport()
        {  }
    }
}
