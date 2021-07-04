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
        public int TransactionId { get; private set; }
        [Timestamp]
        [Required]
        public DateTime DateTime { get; private set; }
        [Required]
        public decimal Price { get; private set; }
        public string Comment { get; private set; }
        [Required]
        public int TransactionType { get; private set; }
        [Required]
        public Category CategoryFk { get; private set; }
        [Required]
        public PaymentType PaymentTypeFk { get; private set; }
    }
}
