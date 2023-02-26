using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDPS.Core.DTOs
{
    public class ParserResultStatus
    {
        public int Errors { get; set; }
        public int ParsedLines { get; set; }
        public string FilePath { get; set; }
    }
}
