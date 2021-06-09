using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        public DatabaseContext DbContext;
        public ObservableCollection<PaymentType> PaymentTypes { get; set; }
        public ApplicationViewModel()
        {
            DbContext = new DatabaseContext();
            DbContext.PaymentTypes.Load();
            PaymentTypes = DbContext.PaymentTypes.Local;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
