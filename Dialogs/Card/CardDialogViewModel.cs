using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VOlkin.Dialogs.Service;

namespace VOlkin.Dialogs.Card
{
    public class CardDialogViewModel : DialogViewModelBase<PaymentType>
    {
        public string _cardNameInput;
        private string _moneyAmountInput;
        private readonly HashSet<string> _existingPtNames;
        private readonly PaymentType _existingPaymentType;

        public CardDialogViewModel(string title, HashSet<string> existingPtNames, PaymentType existingPaymentType = null) : base(title)
        {
            _existingPtNames = existingPtNames;

            OKCommand = new RelayCommand<DialogWindow>(OK);
            CancelCommand = new RelayCommand<DialogWindow>(Cancel);
            OkEnterCommand = new RelayCommand<DialogWindow>(OkEnter);

            if (existingPaymentType != null)
            {
                _existingPaymentType = existingPaymentType;

                CardNameInput = existingPaymentType.TransactionObjectName;
                MoneyAmountInput = existingPaymentType.MoneyAmount.ToString();
            }
        }

        public bool OkButtonAvailable { get; private set; } = false;
        public ICommand OKCommand { get; private set; }
        public ICommand OkEnterCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public string MoneyAmountInput
        {
            get => _moneyAmountInput;
            set
            {
                SetProperty(ref _moneyAmountInput, value);
                UpdateOKbuttonAvailability();
            }
        }

        public string CardNameInput
        {
            get => _cardNameInput;
            set
            {
                SetProperty(ref _cardNameInput, value);
                UpdateOKbuttonAvailability();
            }
        }

        private void UpdateOKbuttonAvailability()
        {
            if (string.IsNullOrWhiteSpace(CardNameInput) || string.IsNullOrWhiteSpace(MoneyAmountInput))
                OkButtonAvailable = false;
            else
                OkButtonAvailable = true;
            OnPropertyChanged("OkButtonAvailable");
        }

        private void Cancel(IDialogWindow window) => CloseDialogWithResult(window, null, false);

        private async void OK(IDialogWindow window)
        {
            if (!decimal.TryParse(MoneyAmountInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal moneyAmount) || moneyAmount <= 0)
            {
                if (Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.IsActive) is MetroWindow metroWindow)
                    await metroWindow.ShowMessageAsync("Ошибка", "Не удалось распознать строку кол-ва средств");

                return;
            }

            if (_existingPtNames.Contains(CardNameInput))
            {
                if (Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.IsActive) is MetroWindow metroWindow)
                    await metroWindow.ShowMessageAsync("Ошибка", "Счет с таким наименованием уже существует");

                return;
            }

            if (_existingPaymentType == null)
            {
                CloseDialogWithResult(window, new PaymentType(CardNameInput, moneyAmount), true);
            }
            else
            {
                _existingPaymentType.Update(CardNameInput, moneyAmount);
                CloseDialog(window, true);
            }

        }

        private void OkEnter(IDialogWindow window)
        {
            if (OkButtonAvailable)
                OK(window);
        }
    }
}
