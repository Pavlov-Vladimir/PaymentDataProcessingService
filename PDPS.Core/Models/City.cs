using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;


namespace PDPS.Core.Models
{
    [Serializable]
    public class City
    {
        [JsonPropertyName("city")]
        public string Name { get; set; }

        [JsonPropertyName("services")]
        public List<Service> Services { get; private set; } = new List<Service>();

        [JsonPropertyName("total")]
        public decimal TotalPayment { get; private set; }

        public City()
        {  }

        public City(string name)
        {
            Name = name;
        }

        public City(string name, List<Service> services) : this(name)
        {
            if (services != null)
            {
                Services = services;
                TotalPayment = services.Sum(s => s.TotalPayment); 
            }
        }

        public City(string name, Service service) : this(name)
        {
            if (service != null)
            {
                Services.Add(service);
                TotalPayment = service.TotalPayment;
            }
        }

        public void AddService(Service service)
        {
            if (service == null) return; 

            if (Services.Exists(s => s.Name == service.Name))
            {
                Services.First(s => s.Name == service.Name).Payers.AddRange(service.Payers);
            }
            else
            {
                Services.Add(service); 
            }
            TotalPayment += service.TotalPayment;
        }

        public void AddServices(List<Service> services)
        {
            foreach (var servise in services)
            {
                AddService(servise);
            }
        }
    }
}
