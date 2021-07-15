using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VOlkin.Dialogs.Service;
using VOlkin.HelpClasses.Enums;

namespace VOlkin.Dialogs.AddTransaction
{
    public class AddTransactionViewModel : DialogViewModelBase<Transaction>
    {
        public AddTransactionViewModel(string title, ObservableCollection<PaymentType> paymentTypes, ObservableCollection<Category> categories) : base(title)
        {
            PaymentTypes = paymentTypes;
            PaymentType = paymentTypes.FirstOrDefault();

            Categories = categories;
            Category = categories.FirstOrDefault();

            OKCommand = new RelayCommand<DialogWindow>(OK);
            CancelCommand = new RelayCommand<DialogWindow>(Cancel);
        }

        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public string Coment { get; set; }
        public DateTime DatTime { get; set; } = DateTime.Now;
        public string Price { get; set; }
        public PaymentType PaymentType { get; set; }
        public ObservableCollection<PaymentType> PaymentTypes { get; set; }

        public Category Category { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        private void Cancel(IDialogWindow window) => CloseDialogWithResult(window, null, false);

        private void OK(IDialogWindow window)
        {
            //TODO: в диалоге ввода выкидывать ошибку, если не парсится, проверки на Null Бесполезны, убрать
            if (!decimal.TryParse(Price, out decimal price) || price <= 0)
            {
                CloseDialogWithResult(window, null, false);
                return;
            }

            Transaction transaction = new(Category, Coment, DatTime, price, PaymentType, TransactionTypeEnum.Expense);

            CloseDialogWithResult(window, transaction, true);
        }
            
    }
}
