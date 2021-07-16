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
using ExtensionMethods;
using VOlkin.HelpClasses;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using VOlkin.Dialogs.ReadTwoDates;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using VOlkin.Dialogs.QuestionDialog;
using VOlkin.Dialogs.AddTransaction;
using System.Collections.Specialized;
using VOlkin.Dialogs.AddCategory;
using VOlkin.HelpClasses.Enums;

namespace VOlkin.ViewModels
{
    public class MainInfoViewModel : ViewModelBase
    {
        private ChangeTimePeriod _setCurTimePeriod;

        private RelayCommand _addCategoryCommand;
        private RelayCommand<Category> _removeCategoryCommand;
        private RelayCommand _addTransactionCommand;
        private RelayCommand<PaymentType> _closeCardCommand;
        private RelayCommand _addCardCommand;

        public MainInfoViewModel()
        {
            DbContext.PaymentTypes.Where(pt => pt.State == StatesEnum.Active).Load();

            DbContext.Categories.Where(ct => ct.State == StatesEnum.Active).Load();

            PaymentTypes.CollectionChanged += (s, e) => { OnPropertyChanged("TotalMoney"); };

            SetCurTimePeriod = TimePeriods.FirstOrDefault();
        }

        public static DatabaseContext DbContext { get; } = new DatabaseContext();
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; } = DateTime.MaxValue;

        public decimal TotalMoney
        {
            get
            {
                return PaymentTypes?.Sum(pt => pt.MoneyAmount) ?? 0;
            }
        }

        public ObservableCollection<PaymentType> PaymentTypes { get; set; } = DbContext.PaymentTypes.Local;
        public ObservableCollection<Category> Categories { get; set; } = DbContext.Categories.Local;
        public ObservableCollection<Transaction> Transactions { get; set; } = DbContext.Transactions.Local;

        public ObservableCollection<ChangeTimePeriod> TimePeriods { get; set; } = new ObservableCollection<ChangeTimePeriod>()
        {
            new ChangeTimePeriod("День", ChangeTimePeriodToDay),
            new ChangeTimePeriod("Неделю", ChangeTimePeriodToWeek),
            new ChangeTimePeriod("Месяц", ChangeTimePeriodToMonth),
            new ChangeTimePeriod("Год", ChangeTimePeriodToYear),
            new ChangeTimePeriod("Все время", ChangeTimePeriodToAllTime),
            new ChangeTimePeriod("Свой период", ChangeTimePeriodToCustom)
        };

        public ChangeTimePeriod SetCurTimePeriod
        {
            get => _setCurTimePeriod;
            set
            {
                if (value.Method())
                {
                    _setCurTimePeriod = value;
                    LoadTransactions();
                }
                OnPropertyChanged("SetCurTimePeriod");
            }
        }

        private static IDialogService DialogService { get; } = new DialogService();

        public RelayCommand AddCategoryCommand => _addCategoryCommand ??= new RelayCommand(AddCategory);
        public RelayCommand AddCardCommand => _addCardCommand ??= new RelayCommand(AddCard);
        public RelayCommand<Category> RemoveCategoryCommand => _removeCategoryCommand ??= new RelayCommand<Category>(RemoveCategory);
        public RelayCommand AddTransactionCommand => _addTransactionCommand ??= new RelayCommand(AddTransaction);
        public RelayCommand<PaymentType> CloseCardCommand => _closeCardCommand ??= new RelayCommand<PaymentType>(CloseCard);

        #region ChangeTimePeriodMethods
        private static bool ChangeTimePeriodToDay() { StartDate = DateTime.Today; EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToWeek() { StartDate = DateTime.Today.FirstDayOfWeek(DayOfWeek.Monday); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToMonth() { StartDate = DateTime.Today.FirstDayOfMonth(); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToYear() { StartDate = DateTime.Today.FirstDayOfYear(); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToAllTime() { StartDate = DateTime.MinValue; EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToCustom()
        {
            if (!DialogService.OpenDialog(new ReadTwoDatesViewModel("Выбор периода времени", StartDate, EndDate), out var readTwoDatesRes))
                return false;

            StartDate = readTwoDatesRes.Item1;
            EndDate = readTwoDatesRes.Item2;

            return true;
        }
        #endregion

        private async void AddCategory()
        {
            if (!DialogService.OpenDialog(new AddCategoryDialogVM("Добавление новой категории"), out var addCategoryDialogRes))
                return;

            if (DbContext.Categories.FirstOrDefault(ct => ct.CategoryType == addCategoryDialogRes.CategoryType && ct.CategoryName == addCategoryDialogRes.CategoryName) != null)
            {
                var metroWindow = Application.Current.MainWindow as MetroWindow;
                await metroWindow.ShowMessageAsync("Ошибка", "Категория с таким наименованием уже существует");

                return;
            }

            DbContext.Categories.Add(addCategoryDialogRes);
            DbContext.SaveChanges();
        }

        private async void RemoveCategory(Category category)
        {
            QuestionDialogView view = new()
            {
                DataContext = new QuestionDialogViewModel($"Вы действительно хотите удалить Категорию \"{category}\"?{Environment.NewLine}" +
                $"Ее нельзя будет восстановить в настройках")
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if ((bool)result == false)
                return;

            category.Delete();
            DbContext.SaveChanges();

            DbContext.Entry(category).State = EntityState.Detached;
        }

        private void AddTransaction()
        {
            if (!DialogService.OpenDialog(new AddTransactionViewModel("Добавление новой транзакции", PaymentTypes, Categories), out var dialogRes))
                return;

            DbContext.Transactions.Add(dialogRes);

            dialogRes.PaymentTypeFk.Decrease(dialogRes.Price);

            DbContext.SaveChanges();
            OnPropertyChanged("TotalMoney");
        }

        private async void CloseCard(PaymentType paymentType)
        {
            QuestionDialogView view = new()
            {
                DataContext = new QuestionDialogViewModel($"Вы действительно хотите закрыть счет \"{paymentType}\"?{Environment.NewLine}" +
                $"Вы всегда сможете восстановить его в настройках")
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if ((bool)result == false)
                return;

            paymentType.Close();
            DbContext.SaveChanges();

            DbContext.Entry(paymentType).State = EntityState.Detached;
        }

        private async void AddCard()
        {
            if (!DialogService.OpenDialog(new AddCardDialogViewModel("Добавление нового счета"), out var addCardDialogRes))
                return;

            if (!decimal.TryParse(addCardDialogRes.Item2, out decimal moneyAmount))
            {
                var metroWindow = Application.Current.MainWindow as MetroWindow;
                await metroWindow.ShowMessageAsync("Ошибка", "Не удалось распознать строку кол-ва средств");

                return;
            }

            if (DbContext.PaymentTypes.FirstOrDefault(pt => pt.PaymentTypeName == addCardDialogRes.Item1) != null)
            {
                var metroWindow = Application.Current.MainWindow as MetroWindow;
                await metroWindow.ShowMessageAsync("Ошибка", "Счет с таким наименованием уже существует");

                return;
            }

            DbContext.PaymentTypes.Add(new PaymentType(addCardDialogRes.Item1, moneyAmount));
            DbContext.SaveChanges();
        }

        private void LoadTransactions()
        {
            DbContext.Transactions.Local.ToList().ForEach(x =>
            {
                DbContext.Entry(x).State = EntityState.Detached;
                x = null;
            });

            DbContext.Transactions.Where(tr => tr.DateTime > StartDate && tr.DateTime <= EndDate).OrderByDescending(tr => tr.DateTime).Load();
            OnPropertyChanged("Transactions");
        }
    }
}

namespace ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfYear(this DateTime value) => new(value.Year, 1, 1);

        public static DateTime FirstDayOfMonth(this DateTime value) => new(value.Year, value.Month, 1);

        public static DateTime FirstDayOfWeek(this DateTime value, DayOfWeek startOfWeek)
        {
            int diff = (7 + (value.DayOfWeek - startOfWeek)) % 7;
            return value.AddDays(-1 * diff).Date;
        }
    }
}