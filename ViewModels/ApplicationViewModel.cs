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
using VOlkin.Dialogs.AddCard;
using VOlkin.Dialogs.Service;
using VOlkin.ViewModels;

namespace VOlkin
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<object> _childrenViews = new() { new OptionsViewModel(), new InvestmentsViewModel(), new MainInfoViewModel(), new CryptoViewModel() };
        public ObservableCollection<object> ChildrenViews { get { return _childrenViews; } }

        public ApplicationViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
