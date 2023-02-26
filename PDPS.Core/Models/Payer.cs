using System;
using System.Text.Json.Serialization;

namespace PDPS.Core.Models
{
    [Serializable]
    public class Payer
    {
        [JsonPropertyName("name")]
        public string FullName { get; set; }

        [JsonPropertyName("payment")]
        public decimal Payment { get; set; }

        [JsonPropertyName("date")]
        public string PaymentDate { get; set; }

        [JsonPropertyName("account_number")]
        public long AccountNumber { get; set; }

        public Payer()
        {  }

        public Payer(string name, decimal payment, string paymentDate, long accountNumber)
        {
            FullName = name;
            Payment = payment;
            PaymentDate = paymentDate;
            AccountNumber = accountNumber;
        }

        public Payer(string name, decimal payment, DateTime paymentDate, long accountNumber)
        {
            FullName = name;
            Payment = payment;
            PaymentDate = paymentDate.Date.ToString("dd-MM-yyyy");
            AccountNumber = accountNumber;
        }
    }
}
