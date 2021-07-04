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
        public override string ToString() => PaymentTypeName;

        [Key]
        public int PaymentTypeId { get; private set; }
        [Required]
        public string PaymentTypeName { get; private set; }
        [Required]
        public decimal MoneyAmount { get; set; }
        [Required]
        public bool IsClosed { get; set; } = false;

        public PaymentType(string ptn, decimal moneyAmount)
        {
            PaymentTypeName = ptn;
            MoneyAmount = moneyAmount;
        }

        private PaymentType()
        { }

        //public Banks BankFk { get; set; }
    }
}
