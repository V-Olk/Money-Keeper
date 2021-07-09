using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin
{
    public class PaymentType : ViewModels.ViewModelBase
    {
        private PaymentType()
        { }

        public PaymentType(string ptn, decimal moneyAmount)
        {
            PaymentTypeName = ptn;
            MoneyAmount = moneyAmount;
        }

        public override string ToString() => PaymentTypeName;

        [Key]
        public int PaymentTypeId { get; private set; }
        [Required]
        public string PaymentTypeName { get; private set; }
        [Required]
        public decimal MoneyAmount { get; private set; }
        [Required]
        public bool IsClosed { get; private set; } = false;

        //public Banks BankFk { get; set; }

        public void Close() => IsClosed = true;

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

    }
}
