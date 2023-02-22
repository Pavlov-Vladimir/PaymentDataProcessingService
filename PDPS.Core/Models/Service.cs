using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PDPS.Core.Models
{
    [Serializable]
    public class Service
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("payers")]
        public List<Payer> Payers { get; private set; } = new List<Payer>();

        [JsonPropertyName("total")]
        public decimal TotalPayment { get; private set; }

        public Service()
        {  }

        public Service(string name)
        {
            Name = name;
        }

        public Service(string name, List<Payer> payers) : this(name)
        {
            if (payers != null)
            {
                Payers = payers;
                TotalPayment = payers.Sum(p => p.Payment); 
            }
        }

        public Service(string name, Payer payer) : this(name)
        {
            if (payer != null)
            {
                Payers.Add(payer);
                TotalPayment = payer.Payment; 
            }
        }

        public void AddPayer(Payer payer)
        {
            if (payer == null) return;

            Payers.Add(payer);
            TotalPayment += payer.Payment;
        }
    }
}
