using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PDPS.Core.Models
{
    [Serializable]
    public class Report
    {
        [JsonPropertyName("report")]
        public List<City> Cities { get; private set; }

        public Report()
        {
            Cities = new List<City>();
        }

        public Report(List<City> cities) : this()
        {
            Cities = cities;
        }

        public void AddCity(City city)
        {
            if (city == null) return;

            if (Cities.Exists(c => c.Name == city.Name))
            {
                Cities.First(c => c.Name == city.Name).AddServices(city.Services);
            }
            else
            {
                Cities.Add(city);
            }
        }
    }
}
