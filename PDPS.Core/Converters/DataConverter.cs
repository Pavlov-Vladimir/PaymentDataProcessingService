using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using PDPS.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDPS.Core.Converters
{
    public class DataConverter : IDataConverter<List<Transaction>, Report>
    {
        public bool IsActive { get; private set; }

        public async Task<Report> Convert(List<Transaction> data)
        {
            if (data == null || data.Count < 1)
                return null;

            IsActive = true;
            Report report = new Report();
            Dictionary<string, List<Service>> cities = new Dictionary<string, List<Service>>();
            try
            {
                await Task.Run(() =>
                {
                    CollectServicesByCity(data, cities);

                    FillReport(report, cities);
                });
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                IsActive = false;
            }

            return report;
        }

        private static void FillReport(Report report, Dictionary<string, List<Service>> cities)
        {
            foreach (var cityName in cities.Keys)
            {
                var city = new City(cityName, cities[cityName]);
                report.AddCity(city);
            }
        }

        private static void CollectServicesByCity(List<Transaction> data, Dictionary<string, List<Service>> cities)
        {
            foreach (var transaction in data)
            {
                var name = $"{transaction.FirstName} {transaction.LastName}";
                var payer = new Payer(name, transaction.Payment, transaction.Date, transaction.AccountNumber);
                var service = new Service(transaction.Service, payer);
                AddServiceIntoDictonary(cities, transaction, payer, service);
            }
        }

        private static void AddServiceIntoDictonary(Dictionary<string, List<Service>> cities, Transaction transaction, Payer payer, Service service)
        {
            if (cities.ContainsKey(transaction.City))
            {
                if (cities[transaction.City].Exists(s => s.Name == service.Name))
                {
                    cities[transaction.City].First(s => s.Name == service.Name).AddPayer(payer);
                }
                else
                {
                    cities[transaction.City].Add(service);
                }
            }
            else
            {
                cities.Add(transaction.City, new List<Service>() { service });
            }
        }
    }
}
