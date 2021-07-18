using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VOlkin.Dialogs.Service;
using VOlkin.HelpClasses.Enums;

namespace VOlkin.Dialogs.AddCategory
{
    public class AddCategoryDialogVM : DialogViewModelBase<Category>
    {
        public string _categoryNameInput;

        public AddCategoryDialogVM(string title) : base(title)
        {
            OKCommand = new RelayCommand<DialogWindow>(OK);
            CancelCommand = new RelayCommand<DialogWindow>(Cancel);
        }

        public CategoryTypeEnum CurrentCategoryType { get; set; }
        public bool OkButtonAvailable { get; private set; } = false;
        public string CategoryNameInput
        {
            get => _categoryNameInput;
            set
            {
                SetProperty(ref _categoryNameInput, value);
                UpdateOKbuttonAvailability();
            }
        }

        public ICommand OKCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void Cancel(IDialogWindow window) => CloseDialogWithResult(window, null, false);

        private void OK(IDialogWindow window) => CloseDialogWithResult(window, new Category(CategoryNameInput, CurrentCategoryType), true);

        private void UpdateOKbuttonAvailability()
        {
            if (string.IsNullOrWhiteSpace(CategoryNameInput))
                OkButtonAvailable = false;
            else
                OkButtonAvailable = true;
            OnPropertyChanged("OkButtonAvailable");
        }
    }
}
