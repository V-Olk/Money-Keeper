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

    }
}
