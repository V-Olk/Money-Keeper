using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.ViewModels;

namespace VOlkin
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Timestamp]
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Comment { get; set; }
        [Required]
        public int TransactionType { get; set; }
        [Required]
        public Category CategoryFk { get; set; }
        [Required]
        public PaymentType PaymentTypeFk { get; set; }
    }
}
