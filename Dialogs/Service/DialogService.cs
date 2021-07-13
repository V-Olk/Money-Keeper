using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin.Dialogs.Service
{
    public class DialogService : IDialogService
    {
        public bool OpenDialog<T>(DialogViewModelBase<T> viewModel, out T result)
        {
            IDialogWindow window = new DialogWindow
            {
                DataContext = viewModel
            };

            window.ShowDialog();

            result = viewModel.DialogResult;

            return (bool)window.DialogResult;
        }
    }
}
