using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses;
using VOlkin.HelpClasses.Enums;
using VOlkin.Models;
using VOlkin.ViewModels;

namespace VOlkin
{
    public class Transaction : NotifyPropertyChanged, ICloneable
    {

        public Transaction(TransactionTypeEnum transactionTypeEnum, decimal price, DateTime dateTime,
                            TransactionObject ptOrCatSource, TransactionObject ptOrCatDestination, string comment = null)
        {
            TransactionType = transactionTypeEnum;
            Price = price;
            DateTime = dateTime;
            SourceFk = ptOrCatSource;
            DestinationFk = ptOrCatDestination;
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

        [Required]
        public TransactionObject SourceFk { get; private set; }

        [Required]
        public TransactionObject DestinationFk { get; private set; }

        public string Comment { get; private set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Changes the amount of money on the payment method
        /// </summary>
        public void Apply()
        {
            switch (TransactionType)
            {
                case TransactionTypeEnum.Expense:
                    (SourceFk as PaymentType)?.Decrease(Price);
                    break;

                case TransactionTypeEnum.Income:
                    (DestinationFk as PaymentType)?.Increase(Price);
                    break;

                case TransactionTypeEnum.Transfer:
                    (SourceFk as PaymentType)?.Decrease(Price);
                    (DestinationFk as PaymentType)?.Increase(Price);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Changes the amount of money on the payment method
        /// </summary>
        public void RollBack()
        {
            switch (TransactionType)
            {
                case TransactionTypeEnum.Expense:
                    (SourceFk as PaymentType)?.Increase(Price);
                    break;

                case TransactionTypeEnum.Income:
                    (DestinationFk as PaymentType)?.Decrease(Price);
                    break;

                case TransactionTypeEnum.Transfer:
                    (SourceFk as PaymentType)?.Increase(Price);
                    (DestinationFk as PaymentType)?.Decrease(Price);
                    break;

                default:
                    break;
            }
        }

        public void Update(decimal price, DateTime dateTime, TransactionObject ptOrCatSource, TransactionObject ptOrCatDestination, string comment)
        {
            Price = price;
            DateTime = dateTime;
            SourceFk = ptOrCatSource;
            DestinationFk = ptOrCatDestination;
            Comment = comment;

            OnPropertyChanged("Price");
            OnPropertyChanged("DateTime");
            OnPropertyChanged("SourceFk");
            OnPropertyChanged("DestinationFk");
            OnPropertyChanged("Comment");
        }
    }
}
