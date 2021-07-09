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

namespace VOlkin.ViewModels
{
    public class MainInfoViewModel : ViewModelBase
    {
        private static readonly IDialogService _dialogService = new DialogService();

        public static DatabaseContext DbContext { get; } = new DatabaseContext();
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

        public decimal TotalMoney
        {
            get
            {
                return PaymentTypes?.Sum(pt => pt.MoneyAmount) ?? 0;
            }
        }
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; } = DateTime.MaxValue;

        private ChangeTimePeriod _setCurTimePeriod;
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

        public MainInfoViewModel()
        {
            DbContext.PaymentTypes.Where(pt => pt.IsClosed == false).Load();

            DbContext.Categories.Load();

            PaymentTypes.CollectionChanged += (s, e) => { OnPropertyChanged("TotalMoney"); };

            SetCurTimePeriod = TimePeriods[0];
        }

        #region AddTransaction

        private RelayCommand _addTransactionCommand;
        public RelayCommand AddTransactionCommand => _addTransactionCommand ??= new RelayCommand(AddTransaction);

        private void AddTransaction()
        {
            var dialog = new AddTransactionViewModel("Добавление новой транзакции", PaymentTypes, Categories);
            var dialogRes = _dialogService.OpenDialog(dialog);

            if (dialogRes == null)
                return;

            DbContext.Transactions.Add(dialogRes);

            PaymentType pt = dialogRes.PaymentTypeFk;
            pt -= dialogRes.Price;

            DbContext.SaveChanges();
            OnPropertyChanged("TotalMoney");
        }

        #endregion

        #region ClosePaymentType
        private RelayCommand<PaymentType> _closeCardCommand;
        public RelayCommand<PaymentType> CloseCardCommand => _closeCardCommand ??= new RelayCommand<PaymentType>(RemoveCard);

        private async void RemoveCard(PaymentType paymentType)
        {
            QuestionDialogView view = new()
            {
                DataContext = new QuestionDialogViewModel($"Вы действительно хотите закрыть счет \"{paymentType.PaymentTypeName}\"?{Environment.NewLine}" +
                $"Вы всегда сможете восстановить его в настройках")
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if ((bool)result == false)
                return;

            paymentType.Close();
            DbContext.SaveChanges();

            DbContext.Entry(paymentType).State = EntityState.Detached;
        }
        #endregion

        #region AddPaymentType

        private RelayCommand _addCardCommand;
        public RelayCommand AddCardCommand => _addCardCommand ??= new RelayCommand(AddCard);

        private async void AddCard()
        {
            var addCardDialog = new AddCardDialogViewModel("Добавление нового счета");
            var addCardDialogRes = _dialogService.OpenDialog(addCardDialog);
            if (addCardDialogRes.Item1 == null || addCardDialogRes.Item2 == null)
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

            PaymentType newPT = new(addCardDialogRes.Item1, moneyAmount);

            DbContext.PaymentTypes.Add(newPT);
            DbContext.SaveChanges();
        }

        #endregion

        #region ChangeTimePeriodMethods
        private static bool ChangeTimePeriodToDay() { StartDate = DateTime.Today; EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToWeek() { StartDate = DateTime.Today.FirstDayOfWeek(DayOfWeek.Monday); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToMonth() { StartDate = DateTime.Today.FirstDayOfMonth(); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToYear() { StartDate = DateTime.Today.FirstDayOfYear(); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToAllTime() { StartDate = DateTime.MinValue; EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToCustom()
        {
            var readTwoDatesDialog = new ReadTwoDatesViewModel("Выбор периода времени", StartDate, EndDate);
            var readTwoDatesRes = _dialogService.OpenDialog(readTwoDatesDialog);
            if (readTwoDatesRes.Item1 == DateTime.MinValue && readTwoDatesRes.Item2 == DateTime.MinValue)
                return false;

            StartDate = readTwoDatesRes.Item1;
            EndDate = readTwoDatesRes.Item2;

            return true;
        }
        #endregion

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