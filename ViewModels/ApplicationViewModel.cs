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
    class ApplicationViewModel : ViewModelBase
    {
        private readonly ObservableCollection<object> _childrenViews = new() { new OptionsViewModel(), new InvestmentsViewModel(), new MainInfoViewModel(), new CryptoViewModel() };
        public ObservableCollection<object> ChildrenViews { get { return _childrenViews; } }

        public ApplicationViewModel()
        {

        }
    }
}
