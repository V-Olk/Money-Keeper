using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VOlkin.Dialogs.Service;

namespace VOlkin.Dialogs.ReadTwoDates
{
    class ReadTwoDatesViewModel : DialogViewModelBase<(DateTime, DateTime)>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; }
        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ReadTwoDatesViewModel(string title, DateTime startDate, DateTime endDate) : base(title)
        {
            StartDate = startDate;

            if (endDate == DateTime.MaxValue)
                EndDate = DateTime.Today.AddDays(1);
            else
                EndDate = endDate;

            OKCommand = new RelayCommand<DialogWindow>(OK);
            CancelCommand = new RelayCommand<DialogWindow>(Cancel);
        }

        private void Cancel(IDialogWindow window) => CloseDialogWithResult(window, (DateTime.MinValue, DateTime.MinValue));

        private void OK(IDialogWindow window) => CloseDialogWithResult(window, (StartDate, EndDate));

        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
