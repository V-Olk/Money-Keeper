using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin
{
    public class PaymentType
    {
        [Key]
        public int PaymentTypeId { get; set; }
        [Required]
        public string PaymentTypeName { get; set; }
        [Required]
        public decimal MoneyAmount { get; set; }
        //public Banks BankFk { get; set; }
    }
}
