using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VOlkin
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        public DatabaseContext DbContext;
        public ObservableCollection<PaymentType> PaymentTypes { get; set; }

        public decimal TotalMoney { get; set; }

        public ApplicationViewModel()
        {
            DbContext = new DatabaseContext();
            DbContext.PaymentTypes.Load();
            PaymentTypes = DbContext.PaymentTypes.Local;
            TotalMoney = PaymentTypes.Sum(pt => pt.MoneyAmount);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private RelayCommand _addCardCommand;
        public RelayCommand AddCardCommand
        {
            get
            {
                return _addCardCommand ??
                  (_addCardCommand = new RelayCommand(AddCard));
            }
        }

        private void AddCard()
        {
            PaymentType newPT = new PaymentType()
            {
                PaymentTypeName = "новый тип оплаты",
                MoneyAmount = 1488
            };

            DbContext.PaymentTypes.Add(newPT);
            DbContext.SaveChanges();
        }
    }
}
