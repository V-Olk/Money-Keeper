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
    public abstract class StateSupport : NotifyPropertyChanged
    {
        [Required]
        public StatesEnum State { get; protected set; }

        public void Close() => State = StatesEnum.Closed;
        public void Delete() => State = StatesEnum.Removed;
        public void MakeActive() => State = StatesEnum.Active;

    }
}
