using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDPS.Core.Parsers
{
    public abstract class ParserBase : IParser<List<Transaction>, ParserResultStatus>
    {
        public ParserResultStatus Status { get; set; }

        public ParserBase() { }

        public ParserBase(string path)
        {
            Status.FilePath = path;
        }

        public virtual async Task<(List<Transaction>, ParserResultStatus)> ParseAsync()
        {
            Status.Errors = 0;
            Status.ParsedLines = 0;

            List<Transaction> transactions = new List<Transaction>();
            string line;
            string[] data;

            using (StreamReader reader = new StreamReader(Status.FilePath))
            {
                PreprocessStreamDependsFromParserType(reader);

                while (!reader.EndOfStream)
                {
                    line = await reader.ReadLineAsync();
                    if (line.Where(c => c == '"').Count() != 2)
                    {
                        Status.Errors++;
                        continue;
                    }

                    data = line.Replace("\"", "").Split(new char[] { ',' });
                    if (data.Length != 9)
                    {
                        Status.Errors++;
                        continue;
                    }

                    bool isValidTransaction = TryCreateTransaction(data, out Transaction transaction);
                    if (!isValidTransaction)
                    {
                        Status.Errors++;
                    }
                    else if (Status.Errors == 0)
                    {
                        transactions.Add(transaction);
                    }
                    Status.ParsedLines++;
                }
            }

            if (Status.Errors != 0)
            {
                transactions = new List<Transaction>();
            }

            return (transactions, Status);
        }

        protected virtual void PreprocessStreamDependsFromParserType(StreamReader reader)
        {
            
        }

        private bool TryCreateTransaction(string[] data, out Transaction transaction)
        {
            if (decimal.TryParse(data[5], out decimal payment) ||
                DateTime.TryParse(data[6], out DateTime date) ||
                long.TryParse(data[7], out long account_number))
            {
                transaction = null;
                return false;
            }

            transaction = new Transaction()
            {
                FirstName = data[0],
                LastName = data[1],
                City = data[2],
                Service = data[8],
                Date = date,
                Payment = payment,
                AccountNumber = account_number
            };
            var context = new ValidationContext(transaction);
            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(transaction, context, results, true);
        }

    }
}
