using PDPS.Core.Contracts;
using PDPS.Core.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDPS.Core.Parsers
{
    public abstract class ParserBase : IParser<List<Transaction>, ParserResultStatus>
    {
        public ParserResultStatus Status { get; set; }
        public List<string> FileContent { get; set; }

        public ParserBase() 
        {
            FileContent = new List<string>();
        }

        public ParserBase(string path) : this()
        {
            Status = new ParserResultStatus()
            {
                FilePath = path
            };
        }

        public async Task ReadFileAsync()
        {
            FileContent = await Task.Run(async () =>
            {
                await Task.Delay(1);                    // because FileSystemWatcher some times hasn't released the file yet 
                return File.ReadLines(Status.FilePath).ToList();
            });
        }

        public virtual async Task<(List<Transaction>, ParserResultStatus)> ParseAsync()
        {
            PrepareContentDependsByParserType();

            Status.Errors = 0;
            Status.ParsedLines = 0;

            List<Transaction> transactions = new List<Transaction>();
            string[] data;

            foreach (string line in FileContent)
            {
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
                var transactionData = data.Select(x => x.Trim()).ToArray();

                bool isValidTransaction = TryCreateTransaction(transactionData, out Transaction transaction);
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

            if (Status.Errors != 0)
            {
                transactions = new List<Transaction>();
            }

            return (transactions, Status);
        }

        protected virtual void PrepareContentDependsByParserType()
        {  }

        private bool TryCreateTransaction(string[] data, out Transaction transaction)
        {
            bool isDecimal = decimal.TryParse(data[5], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal payment);
            bool isDate = DateTime.TryParseExact(data[6], "yyyy-dd-MM", null, DateTimeStyles.None, out DateTime date);
            bool isLong = long.TryParse(data[7], out long account_number);
            bool isValidFields = isDecimal && isDate && isLong;
            if (!isValidFields)
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
