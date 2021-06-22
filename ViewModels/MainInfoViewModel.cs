using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VOlkin.Dialogs.AddCard;
using VOlkin.Dialogs.Service;

namespace VOlkin.ViewModels
{
    public class MainInfoViewModel : INotifyPropertyChanged
    {
        private readonly IDialogService _dialogService = new DialogService();

        public DatabaseContext DbContext;
        public ObservableCollection<PaymentType> PaymentTypes { get; set; }
        public ObservableCollection<Transaction> Transactions { get; set; }
        public decimal TotalMoney { get; set; }

        public MainInfoViewModel()
        {
            DbContext = new DatabaseContext();
            DbContext.PaymentTypes.Load();
            PaymentTypes = DbContext.PaymentTypes.Local;

            DbContext.Transactions.OrderByDescending(tr => tr.DateTime).Load();//.Where(tr => tr.DateTime > DateTime.Today).Load();
            Transactions = DbContext.Transactions.Local;

            TotalMoney = PaymentTypes.Sum(pt => pt.MoneyAmount);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #region AddPaymentType

        private RelayCommand _addCardCommand;
        public RelayCommand AddCardCommand
        {
            get { return _addCardCommand ??= new RelayCommand(AddCard); }
        }

        private void AddCard()
        {
            var addCardDialog = new AddCardDialogViewModel("Добавление нового счета");
            var addCardDialogRes = _dialogService.OpenDialog(addCardDialog);
            if (addCardDialogRes.Item1 == null || addCardDialogRes.Item2 == null)
                return;

            if (!decimal.TryParse(addCardDialogRes.Item2, out decimal moneyAmount))
                throw new Exception("Не удалось распознать строку кол-ва средств");

            if (DbContext.PaymentTypes.FirstOrDefault(pt => pt.PaymentTypeName == addCardDialogRes.Item1) != null)
                throw new Exception("Счет с таким наименованием уже существует");

            PaymentType newPT = new()
            {
                PaymentTypeName = addCardDialogRes.Item1,
                MoneyAmount = moneyAmount
            };

            DbContext.PaymentTypes.Add(newPT);
            DbContext.SaveChanges();

            ////TODO: it needs to be automated?
            TotalMoney += moneyAmount;
            OnPropertyChanged("TotalMoney");
        }

        #endregion
    }
}
