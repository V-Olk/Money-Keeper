using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses.Enums;
using VOlkin.ViewModels;

namespace VOlkin
{
    public class Transaction
    {
        public Transaction(Category category, string comment, DateTime dateTime, decimal price, PaymentType paymentType, TransactionTypeEnum transactionTypeEnum)
        {
            CategoryFk = category;
            Comment = comment;
            DateTime = dateTime;
            Price = price;
            PaymentTypeFk = paymentType;
            TransactionType = transactionTypeEnum;
        }
        private Transaction() { }

        [Key]
        public int TransactionId { get; private set; }
        [Timestamp]
        [Required]
        public DateTime DateTime { get; private set; }
        [Required]
        public decimal Price { get; private set; }
        [Required]
        public TransactionTypeEnum TransactionType { get; private set; }
        [Required]
        public Category CategoryFk { get; private set; }
        [Required]
        public PaymentType PaymentTypeFk { get; private set; }
        public string Comment { get; private set; }

    }
}
