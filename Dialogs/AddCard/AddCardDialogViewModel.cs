using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VOlkin.Dialogs.Service;

namespace VOlkin.Dialogs.AddCard
{
    public class AddCardDialogViewModel : DialogViewModelBase<(string, string)>
    {
        public string _cardNameInput;

        public AddCardDialogViewModel(string title) : base(title)
        {
            OKCommand = new RelayCommand<DialogWindow>(OK);
            CancelCommand = new RelayCommand<DialogWindow>(Cancel);
            OkEnterCommand = new RelayCommand<DialogWindow>(OkEnter);
        }

        public bool OkButtonAvailable { get; private set; } = false;
        public ICommand OKCommand { get; private set; }
        public ICommand OkEnterCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public string MoneyAmountInput { get; set; }

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
            if (string.IsNullOrWhiteSpace(CardNameInput))
                OkButtonAvailable = false;
            else
                OkButtonAvailable = true;
            OnPropertyChanged("OkButtonAvailable");
        }

        private void Cancel(IDialogWindow window) => CloseDialogWithResult(window, (null, null), false);

        private void OK(IDialogWindow window) => CloseDialogWithResult(window, (CardNameInput, MoneyAmountInput), true);

        private void OkEnter(IDialogWindow window)
        { 
            if (OkButtonAvailable)
                CloseDialogWithResult(window, (CardNameInput, MoneyAmountInput), true);
        }
    }
}
