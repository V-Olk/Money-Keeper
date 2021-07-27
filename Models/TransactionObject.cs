using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOlkin.HelpClasses;
using VOlkin.HelpClasses.Enums;

namespace VOlkin.Models
{
    public abstract class TransactionObject : NotifyPropertyChanged
    {
        [Key]
        public int TransactionObjectId { get; private set; }

        [Required]
        public string TransactionObjectName { get; protected set; }

        [Required]
        public StatesEnum State { get; protected set; }

        public void Close() => State = StatesEnum.Closed;
        public void Delete() => State = StatesEnum.Removed;
        public void MakeActive() => State = StatesEnum.Active;

        public override string ToString() => TransactionObjectName;
    }
}
