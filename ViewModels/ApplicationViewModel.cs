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
using VOlkin.Dialogs.Card;
using VOlkin.Dialogs.Service;
using VOlkin.ViewModels;

namespace VOlkin
{
    class ApplicationViewModel : ViewModelBase
    {
        private RelayCommand<int> _selectedTabChangedCommand;

        public ApplicationViewModel()
        {
            LoadTabViewModel(2);//TODO: Change value to value saved in settings like default tab
        }

        private readonly ObservableCollection<object> _childrenViews = new() { null, null, null, null };
        public ObservableCollection<object> ChildrenViews { get { return _childrenViews; } }
        public RelayCommand<int> SelectedTabChangedCommand => _selectedTabChangedCommand ??= new RelayCommand<int>(LoadTabViewModel);

        private void LoadTabViewModel(int selectedTabIndex)
        {
            if (ChildrenViews[selectedTabIndex] == null)
            {
                switch (selectedTabIndex)
                {
                    case 0:
                        ChildrenViews[selectedTabIndex] = new OptionsViewModel();
                        break;

                    case 1:
                        ChildrenViews[selectedTabIndex] = new InvestmentsViewModel();
                        break;

                    case 2:
                        ChildrenViews[selectedTabIndex] = new MainInfoViewModel();
                        break;

                    case 3:
                        ChildrenViews[selectedTabIndex] = new CryptoViewModel();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
