using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VOlkin.Dialogs.Service;
using VOlkin.HelpClasses.Enums;

namespace VOlkin.Dialogs.AddCategory
{
    public class AddCategoryDialogVM : DialogViewModelBase<Category>
    {
        private string _categoryNameInput;
        private readonly HashSet<(string, CategoryTypeEnum)> _existingCategoriesNameAndType;

        public AddCategoryDialogVM(string title, HashSet<(string, CategoryTypeEnum)> existingCategoriesNameAndType) : base(title)
        {
            _existingCategoriesNameAndType = existingCategoriesNameAndType;

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

        private async void OK(IDialogWindow window)
        {
            if (_existingCategoriesNameAndType.Contains((CategoryNameInput, CurrentCategoryType)))
            {
                if (Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.IsActive) is MetroWindow metroWindow)
                    await metroWindow.ShowMessageAsync("Ошибка", "Категория с таким наименованием уже существует");

                return;
            }

            CloseDialogWithResult(window, new Category(CategoryNameInput, CurrentCategoryType), true);
        }

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
