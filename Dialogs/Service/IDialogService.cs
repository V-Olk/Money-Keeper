using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOlkin.Dialogs.Service
{
    public interface IDialogService
    {
        bool OpenInputDialog<T>(DialogViewModelBase<T> viewModel, out T result);

        bool OpenDialog<T>(DialogViewModelBase<T> viewModel);
    }
}
