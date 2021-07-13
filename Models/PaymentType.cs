using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses;

namespace VOlkin
{
    public class PaymentType : NotifyPropertyChanged
    {
        private PaymentType() { }

        public PaymentType(string ptn, decimal moneyAmount)
        {
            PaymentTypeName = ptn;
            MoneyAmount = moneyAmount;
        }

        [Key]
        public int PaymentTypeId { get; private set; }
        [Required]
        public string PaymentTypeName { get; private set; }
        [Required]
        public decimal MoneyAmount { get; private set; }
        [Required]
        public bool IsClosed { get; private set; } = false;

        //public Banks BankFk { get; set; }

        public static PaymentType operator +(PaymentType pt, decimal moneyAmount)
        {
            pt.MoneyAmount += moneyAmount;
            return pt;
        }

        public static PaymentType operator -(PaymentType pt, decimal moneyAmount)
        {
            pt.MoneyAmount -= moneyAmount;
            pt.OnPropertyChanged("MoneyAmount");
            return pt;
        }

        public void Close() => IsClosed = true;

        public void Increase(decimal moneyAmount)
        {
            MoneyAmount += moneyAmount;
        }

        public void Decrease(decimal moneyAmount)
        {
            MoneyAmount -= moneyAmount;
        }

        public override string ToString() => PaymentTypeName;
    }
}
