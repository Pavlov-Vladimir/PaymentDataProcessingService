using System;
using System.ComponentModel.DataAnnotations;

namespace PDPS.Core.DTOs
{
    public class Transaction
    {
        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string City { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string Service { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Payment { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public long AccountNumber { get; set; }
    }
}
