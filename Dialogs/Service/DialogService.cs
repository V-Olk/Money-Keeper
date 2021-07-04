using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin.Dialogs.Service
{
    public class DialogService : IDialogService
    {
        public T OpenDialog<T>(DialogViewModelBase<T> viewModel)
        {
            IDialogWindow window = new DialogWindow
            {
                DataContext = viewModel
            };

            window.ShowDialog();

            return viewModel.DialogResult;
        }
    }
}
