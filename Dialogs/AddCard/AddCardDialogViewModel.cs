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
    class AddCardDialogViewModel : DialogViewModelBase<(string, string)>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string CardNameInput { get; set; }
        public string MoneyAmountInput { get; set; }
        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public AddCardDialogViewModel(string title) : base(title)
        {
            OKCommand = new RelayCommand<DialogWindow>(OK);
            CancelCommand = new RelayCommand<DialogWindow>(Cancel);
        }

        private void Cancel(IDialogWindow window)
        {
            CloseDialogWithResult(window, (null, null));
        }

        private void OK(IDialogWindow window)
        {
            CloseDialogWithResult(window, (CardNameInput, MoneyAmountInput));
        }
    }
}
