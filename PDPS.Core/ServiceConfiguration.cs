using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDPS.Core
{
    public class ServiceConfiguration
    {
        private readonly ConfigurationManager _configurationManager = new ConfigurationManager();
        public ConfigurationManager Manager { get; }
        public string InputFolder { get; private set; }
        public string OutputFolder { get; private set; }

        public ServiceConfiguration() 
        {
            Manager= new ConfigurationManager();
            Manager.AddConfiguration(_configurationManager);
        }

        public void SetConfiguration()
        {
            InputFolder = Manager.GetSection("InitialConnection").Value;
            OutputFolder = Manager.GetSection("DefaultConnection").Value;
        }
    }
}
