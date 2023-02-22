using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using PDPS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDPS.Core.Converters
{
    public class DataConverter : IDataConverter<List<Transaction>, Report>
    {
        public async Task<Report> Convert(List<Transaction> data)
        {
            Report report = new Report();

            await Task.Run(() =>
            {
                var payer = new Payer();

                foreach (var transaction in data)
                {
                    SetPayerFilds(payer, transaction);

                    var service = new Service(transaction.Service, payer);

                    var city = new City(transaction.City, service);

                    report.AddCity(city);
                }
            });

            return report;
        }

        private static void SetPayerFilds(Payer payer, Transaction transaction)
        {
            var name = $"{transaction.FirstName} {transaction.LastName}";
            payer.FullName = name;
            payer.AccountNumber = transaction.AccountNumber;
            payer.Payment = transaction.Payment;
            payer.PaymentDate = transaction.Date.ToString("dd-MM-yyyy");
        }
    }
}
