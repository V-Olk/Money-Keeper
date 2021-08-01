using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VOlkin.Dialogs.Service;
using VOlkin.HelpClasses.Enums;
using VOlkin.Models;

namespace VOlkin.Dialogs.TransactionDialog
{
    public class TransactionDialogVM : DialogViewModelBase<Transaction>
    {
        private TransactionTypeEnum _currentTransactionType;

        private readonly string _textFromPaymentLocalized;
        private readonly string _textFromCategoryLocalized;
        private readonly string _textToPaymentLocalized;
        private readonly string _textToCategoryLocalized;

        private readonly ObservableCollection<PaymentType> _paymentTypes;
        private readonly ObservableCollection<Category> _categories;

        private TransactionObject _currentPTorCatFrom;
        private string _price;

        private readonly Transaction _existingTransaction;

        public TransactionDialogVM(string title, ObservableCollection<PaymentType> paymentTypes, ObservableCollection<Category> categories, Transaction existingTransaction = null) : base(title)
        {
            _paymentTypes = new ObservableCollection<PaymentType>(paymentTypes);
            _categories = new ObservableCollection<Category>(categories);

            _textFromPaymentLocalized = "Со счета";
            _textFromCategoryLocalized = "С категории";

            _textToPaymentLocalized = "на счет";
            _textToCategoryLocalized = "на категорию";

            OKCommand = new RelayCommand<DialogWindow>(OK);
            CancelCommand = new RelayCommand<DialogWindow>(Cancel);
            OkEnterCommand = new RelayCommand<DialogWindow>(OkEnter);

            if (existingTransaction != null)
            {
                _existingTransaction = existingTransaction;

                TransactionTypeComboBoxIsEnabled = false;

                AddTransactionObjectToObservableIfNotContain(existingTransaction.SourceFk);
                AddTransactionObjectToObservableIfNotContain(existingTransaction.DestinationFk);

                CurrentTransactionType = existingTransaction.TransactionType;

                Price = existingTransaction.Price.ToString();
                DatTime = existingTransaction.DateTime;
                Comment = existingTransaction.Comment;
            }
            else
            {
                CurrentTransactionType = TransactionTypeEnum.Expense;
            }
        }

        public TransactionTypeEnum CurrentTransactionType
        {
            get => _currentTransactionType;
            set
            {
                SetProperty(ref _currentTransactionType, value);

                switch (value)
                {
                    case TransactionTypeEnum.Expense:
                        TextFrom = _textFromPaymentLocalized;
                        TextTo = _textToCategoryLocalized;

                        PTorCatFrom = new ObservableCollection<TransactionObject>(_paymentTypes);
                        PTorCatTo = new ObservableCollection<TransactionObject>(_categories.Where(ct => ct.CategoryType == CategoryTypeEnum.Expense));

                        break;

                    case TransactionTypeEnum.Income:
                        TextFrom = _textFromCategoryLocalized;
                        TextTo = _textToPaymentLocalized;

                        PTorCatFrom = new ObservableCollection<TransactionObject>(_categories.Where(ct => ct.CategoryType == CategoryTypeEnum.Income));
                        PTorCatTo = new ObservableCollection<TransactionObject>(_paymentTypes);

                        break;

                    case TransactionTypeEnum.Transfer:
                        TextFrom = _textFromPaymentLocalized;
                        TextTo = _textToPaymentLocalized;

                        PTorCatFrom = new ObservableCollection<TransactionObject>(_paymentTypes);
                        PTorCatTo = new ObservableCollection<TransactionObject>(_paymentTypes.Skip(1));

                        break;

                    default:
                        break;
                }

                if (_existingTransaction == null)
                {
                    CurrentPTorCatFrom = PTorCatFrom.FirstOrDefault();
                    CurrentPTorCatTo = PTorCatTo.FirstOrDefault();
                }
                else
                {
                    CurrentPTorCatFrom = PTorCatFrom.FirstOrDefault(trObj => trObj.TransactionObjectId == _existingTransaction.SourceFk.TransactionObjectId);
                    CurrentPTorCatTo = PTorCatTo.FirstOrDefault(trObj => trObj.TransactionObjectId == _existingTransaction.DestinationFk.TransactionObjectId);
                }

                OnPropertyChanged("CurrentPTorCatFrom");
                OnPropertyChanged("PTorCatFrom");

                OnPropertyChanged("CurrentPTorCatTo");
                OnPropertyChanged("PTorCatTo");

                OnPropertyChanged("TextFrom");
                OnPropertyChanged("TextTo");

            }

        }

        public string Comment { get; set; }
        public DateTime DatTime { get; set; } = DateTime.Now;

        public string Price
        {
            get => _price;
            set
            {
                SetProperty(ref _price, value);
                UpdateOKbuttonAvailability();
            }
        }

        public string TextFrom { get; set; }
        public string TextTo { get; set; }

        public ObservableCollection<TransactionObject> PTorCatFrom { get; set; }
        public ObservableCollection<TransactionObject> PTorCatTo { get; set; }

        public TransactionObject CurrentPTorCatFrom
        {
            get => _currentPTorCatFrom;
            set
            {
                SetProperty(ref _currentPTorCatFrom, value);

                if (CurrentTransactionType == TransactionTypeEnum.Transfer)
                {
                    PTorCatTo = new ObservableCollection<TransactionObject>(_paymentTypes.Where(pt => pt != CurrentPTorCatFrom));

                    if (CurrentPTorCatTo == CurrentPTorCatFrom)
                    {
                        CurrentPTorCatTo = PTorCatTo.FirstOrDefault();
                        OnPropertyChanged("CurrentPTorCatTo");
                    }

                    OnPropertyChanged("PTorCatTo");
                }
            }
        }

        public TransactionObject CurrentPTorCatTo { get; set; }
        public bool OkButtonAvailable { get; private set; } = false;
        public bool TransactionTypeComboBoxIsEnabled { get; private set; } = true;

        public ICommand OKCommand { get; private set; }
        public ICommand OkEnterCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void UpdateOKbuttonAvailability()
        {
            if (string.IsNullOrWhiteSpace(Price))
                OkButtonAvailable = false;
            else
                OkButtonAvailable = true;
            OnPropertyChanged("OkButtonAvailable");
        }

        private void Cancel(IDialogWindow window) => CloseDialog(window, false);

        private async void OK(IDialogWindow window)
        {
            if (!decimal.TryParse(Price, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
            {
                if (Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.IsActive) is MetroWindow metroWindow)
                    await metroWindow.ShowMessageAsync("Ошибка", "Не удалось распознать строку кол-ва средств");

                return;
            }

            if (_existingTransaction == null)
            {
                Transaction transaction = new(CurrentTransactionType, price, DatTime, CurrentPTorCatFrom, CurrentPTorCatTo, Comment);
                CloseDialogWithResult(window, transaction, true);
            }
            else
            {
                _existingTransaction.Update(price, DatTime, CurrentPTorCatFrom, CurrentPTorCatTo, Comment);
                CloseDialog(window, true);
            }
        }

        /// <summary>
        /// Add TransactionObject to Observable (if this TransactionObject was disabled or deleted)
        /// </summary>
        private void AddTransactionObjectToObservableIfNotContain(TransactionObject trObj)
        {
            if (trObj is PaymentType pt)
            {
                if (!_paymentTypes.Contains(pt))
                    _paymentTypes.Add(pt);
            }
            else if (trObj is Category ct)
            {
                if (_categories.Contains(ct))
                    _categories.Add(ct);
            }
        }

        private void OkEnter(IDialogWindow window)
        {
            if (OkButtonAvailable)
                OK(window);
        }
    }
}
