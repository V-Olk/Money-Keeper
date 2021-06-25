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

namespace VOlkin.ViewModels
{
    public class MainInfoViewModel : INotifyPropertyChanged
    {
        private readonly IDialogService _dialogService = new DialogService();

        public DatabaseContext DbContext;
        public ObservableCollection<PaymentType> PaymentTypes { get; set; }

        public ObservableCollection<Transaction> Transactions { get; set; }

        public ObservableCollection<ChangeTimePeriod> TimePeriods { get; set; } = new ObservableCollection<ChangeTimePeriod>()
        {
            new ChangeTimePeriod("День", ChangeTimePeriodToDay),
            new ChangeTimePeriod("Неделю", ChangeTimePeriodToWeek),
            new ChangeTimePeriod("Месяц", ChangeTimePeriodToMonth),
            new ChangeTimePeriod("Год", ChangeTimePeriodToYear),
            new ChangeTimePeriod("Все время", ChangeTimePeriodToAllTime),
            new ChangeTimePeriod("Свой период", ChangeTimePeriodToCustom)
        };

        public decimal TotalMoney { get; set; }
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; } = DateTime.MaxValue;//default max val

        private ChangeTimePeriod _setCurTimePeriod;
        public ChangeTimePeriod SetCurTimePeriod
        {
            get
            {
                return _setCurTimePeriod;
            }
            set
            {
                _setCurTimePeriod = value;
                _setCurTimePeriod.Method();
                LoadTransactions();
            }
        }

        public MainInfoViewModel()
        {
            DbContext = new DatabaseContext();
            DbContext.PaymentTypes.Load();
            PaymentTypes = DbContext.PaymentTypes.Local;
            Transactions = DbContext.Transactions.Local;

            TotalMoney = PaymentTypes.Sum(pt => pt.MoneyAmount);
            SetCurTimePeriod = TimePeriods[0];
        }

        #region AddPaymentType

        private RelayCommand _addCardCommand;
        public RelayCommand AddCardCommand
        {
            get { return _addCardCommand ??= new RelayCommand(AddCard); }
        }

        private void AddCard()
        {
            var addCardDialog = new AddCardDialogViewModel("Добавление нового счета");
            var addCardDialogRes = _dialogService.OpenDialog(addCardDialog);
            if (addCardDialogRes.Item1 == null || addCardDialogRes.Item2 == null)
                return;

            if (!decimal.TryParse(addCardDialogRes.Item2, out decimal moneyAmount))
                throw new Exception("Не удалось распознать строку кол-ва средств");

            if (DbContext.PaymentTypes.FirstOrDefault(pt => pt.PaymentTypeName == addCardDialogRes.Item1) != null)
                throw new Exception("Счет с таким наименованием уже существует");

            PaymentType newPT = new()
            {
                PaymentTypeName = addCardDialogRes.Item1,
                MoneyAmount = moneyAmount
            };

            DbContext.PaymentTypes.Add(newPT);
            DbContext.SaveChanges();

            ////TODO: it needs to be automated?
            TotalMoney += moneyAmount;
            OnPropertyChanged("TotalMoney");
        }

        #endregion

        #region ChangeTimePeriodMethods
        private static void ChangeTimePeriodToDay() { StartDate = DateTime.Today; EndDate = DateTime.MaxValue; }
        private static void ChangeTimePeriodToWeek() { StartDate = DateTime.Today.FirstDayOfWeek(DayOfWeek.Monday); EndDate = DateTime.MaxValue; }
        private static void ChangeTimePeriodToMonth() { StartDate = DateTime.Today.FirstDayOfMonth(); EndDate = DateTime.MaxValue; }
        private static void ChangeTimePeriodToYear() { StartDate = DateTime.Today.FirstDayOfYear(); EndDate = DateTime.MaxValue; }
        private static void ChangeTimePeriodToAllTime() { StartDate = DateTime.MinValue; EndDate = DateTime.MaxValue; }
        private static void ChangeTimePeriodToCustom()
        {
            //TODO: custom dialog that read two dates
            throw new NotImplementedException();
        }
        #endregion

        private void LoadTransactions()
        {
            //DbContext.Transactions.Local.Clear();

            DbContext.Transactions.Local.ToList().ForEach(x =>
            {
                DbContext.Entry(x).State = EntityState.Detached;
                x = null; // this doesn't seem to be required for garbage collection
            });

            DbContext.Transactions.Where(tr => tr.DateTime > StartDate && tr.DateTime <= EndDate).OrderByDescending(tr => tr.DateTime).Load();
            OnPropertyChanged("Transactions");
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}

namespace ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1);
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime FirstDayOfWeek(this DateTime value, DayOfWeek startOfWeek)
        {
            int diff = (7 + (value.DayOfWeek - startOfWeek)) % 7;
            return value.AddDays(-1 * diff).Date;
        }
    }
}