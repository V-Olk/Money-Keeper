using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses.Enums;
using VOlkin.Models;
using VOlkin.ViewModels;

namespace VOlkin
{
    public class Transaction
    {

        public Transaction(TransactionTypeEnum transactionTypeEnum, decimal price, DateTime dateTime,
                            StateSupport ptOrCatSource, StateSupport ptOrCatDestination, string comment = null)
        {
            TransactionType = transactionTypeEnum;
            Price = price;
            DateTime = dateTime;

            switch (transactionTypeEnum)
            {
                case TransactionTypeEnum.Expense:
                    PaymentTypeSourceFk = ptOrCatSource as PaymentType;
                    CategoryFk = ptOrCatDestination as Category;
                    break;

                case TransactionTypeEnum.Income:
                    CategoryFk = ptOrCatSource as Category;
                    PaymentTypeDestFk = ptOrCatDestination as PaymentType;
                    break;

                case TransactionTypeEnum.Transfer:
                    PaymentTypeSourceFk = ptOrCatSource as PaymentType;
                    PaymentTypeDestFk = ptOrCatDestination as PaymentType;
                    break;

                default:
                    break;
            }

            Comment = comment;
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
        public Category CategoryFk { get; private set; }
        public PaymentType PaymentTypeSourceFk { get; private set; }
        public PaymentType PaymentTypeDestFk { get; private set; }
        public string Comment { get; private set; }

    }
}
