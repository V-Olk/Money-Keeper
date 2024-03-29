﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using VOlkin.Dialogs.CardDialog;
using VOlkin.Dialogs.Service;
using ExtensionMethods;
using VOlkin.HelpClasses;
using VOlkin.Dialogs.ReadTwoDates;
using MaterialDesignThemes.Wpf;
using VOlkin.Dialogs.QuestionDialog;
using VOlkin.Dialogs.TransactionDialog;
using VOlkin.Dialogs.CategoryDialog;
using VOlkin.HelpClasses.Enums;
using VOlkin.Models;
using System.Windows.Data;

namespace VOlkin.ViewModels
{
    public class MainInfoViewModel : ViewModelBase
    {
        private ChangeTimePeriod _setCurTimePeriod;

        private RelayCommand _addCardCommand;
        private RelayCommand _addCategoryCommand;
        private RelayCommand _addTransactionCommand;

        private RelayCommand<PaymentType> _updateCardCommand;
        private RelayCommand<Category> _updateCategoryCommand;
        private RelayCommand<Transaction> _updateTransactionCommand;

        private RelayCommand<TransactionObject> _closeStateSupportObjCommand;

        private RelayCommand<TransactionObject> _removeStateSupportObjCommand;
        private RelayCommand<Transaction> _removeTransactionCommand;

        public MainInfoViewModel()
        {
            DbContext.PaymentTypes.Where(pt => pt.State == StatesEnum.Active).Load();

            DbContext.Categories.Where(ct => ct.State == StatesEnum.Active).Load();

            PaymentTypes.CollectionChanged += (s, e) => { OnPropertyChanged("TotalMoney"); };
            Transactions.CollectionChanged += (s, e) => { OnPropertyChanged("TotalMoney"); };

            SetCurTimePeriod = TimePeriods.FirstOrDefault();
        }

        public static DatabaseContext DbContext { get; } = new DatabaseContext();
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; } = DateTime.MaxValue;

        public decimal TotalMoney => PaymentTypes?.Where(pt => pt.State == StatesEnum.Active).Sum(pt => pt.MoneyAmount) ?? 0;

        public ObservableCollection<Category> Categories { get; set; } = DbContext.Categories.Local;
        public ObservableCollection<PaymentType> PaymentTypes { get; set; } = DbContext.PaymentTypes.Local;
        public ObservableCollection<Transaction> Transactions { get; set; } = DbContext.Transactions.Local;

        public ICollectionView FilteredPaymentTypes
        {
            get
            {
                ICollectionView source = CollectionViewSource.GetDefaultView(PaymentTypes);
                source.Filter = paymentType => FilterPaymentType((PaymentType)paymentType);
                return source;
            }
        }

        public ICollectionView FilteredCategories
        {
            get
            {
                ICollectionView source = CollectionViewSource.GetDefaultView(Categories);
                source.Filter = category => FilterCategory((Category)category);
                return source;
            }
        }

        public ICollectionView FilteredTransactions
        {
            get
            {
                ICollectionView source = CollectionViewSource.GetDefaultView(Transactions);
                source.Filter = tr => FilterTransaction((Transaction)tr);
                return source;
            }
        }

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

        public RelayCommand AddCardCommand => _addCardCommand ??= new(AddCard);
        public RelayCommand AddCategoryCommand => _addCategoryCommand ??= new(AddCategory);
        public RelayCommand AddTransactionCommand => _addTransactionCommand ??= new(AddTransaction);

        public RelayCommand<TransactionObject> CloseStateSupportObjCommand => _closeStateSupportObjCommand ??= new(CloseStateSupportObj);
        public RelayCommand<TransactionObject> RemoveStateSupportObjCommand => _removeStateSupportObjCommand ??= new(RemoveStateSupportObj);
        public RelayCommand<Transaction> RemoveTransactionCommand => _removeTransactionCommand ??= new(RemoveTransaction);

        public RelayCommand<PaymentType> UpdateCardCommand => _updateCardCommand ??= new(UpdateCard);
        public RelayCommand<Category> UpdateCategoryCommand => _updateCategoryCommand ??= new(UpdateCategory);
        public RelayCommand<Transaction> UpdateTransactionCommand => _updateTransactionCommand ??= new(UpdateTransaction);


        #region ChangeTimePeriodMethods
        private static bool ChangeTimePeriodToDay() { StartDate = DateTime.Today; EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToWeek() { StartDate = DateTime.Today.FirstDayOfWeek(DayOfWeek.Monday); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToMonth() { StartDate = DateTime.Today.FirstDayOfMonth(); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToYear() { StartDate = DateTime.Today.FirstDayOfYear(); EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToAllTime() { StartDate = DateTime.MinValue; EndDate = DateTime.MaxValue; return true; }
        private static bool ChangeTimePeriodToCustom()
        {
            if (!DialogService.OpenInputDialog(new ReadTwoDatesViewModel("Выбор периода времени", StartDate, EndDate), out var readTwoDatesRes))
                return false;

            StartDate = readTwoDatesRes.Item1;
            EndDate = readTwoDatesRes.Item2;

            return true;
        }
        #endregion

        private void AddCard()
        {
            if (!DialogService.OpenInputDialog(new CardDialogViewModel("Добавление нового счета",
                                                                     DbContext.PaymentTypes
                                                                     .Where(pt => pt.State == StatesEnum.Active || pt.State == StatesEnum.Closed)
                                                                     .Select(pt => pt.TransactionObjectName)
                                                                     .ToHashSet()),
                                                                     out PaymentType newPaymentType))
                return;

            DbContext.PaymentTypes.Add(newPaymentType);
            DbContext.SaveChanges();
        }

        private void AddCategory()
        {
            if (!DialogService.OpenInputDialog(new CategoryDialogVM("Добавление новой категории",
                                                                   DbContext.Categories
                                                                     .Where(ct => ct.State == StatesEnum.Active || ct.State == StatesEnum.Closed)
                                                                     .AsEnumerable()
                                                                     .Select(ct => (ct.TransactionObjectName, ct.CategoryType))
                                                                     .ToHashSet()),
                                                                   out Category newCategory))
                return;

            DbContext.Categories.Add(newCategory);
            DbContext.SaveChanges();
        }

        private void AddTransaction()
        {
            if (!DialogService.OpenInputDialog(new TransactionDialogVM("Добавление новой транзакции",
                                               new ObservableCollection<PaymentType>(PaymentTypes.Where(pt => pt.State == StatesEnum.Active)),
                                               new ObservableCollection<Category>(Categories.Where(ct => ct.State == StatesEnum.Active))),
                                               out Transaction newTransaction))
                return;

            DbContext.Transactions.Add(newTransaction);

            newTransaction.Apply();

            DbContext.SaveChanges();

            LoadTransactions();
        }

        private void LoadTransactions()
        {
            DbContext.Transactions
                        .Where(tr => tr.DateTime > StartDate && tr.DateTime <= EndDate)
                        .Include(tr => tr.SourceFk)
                        .Include(tr => tr.DestinationFk)
                        .Load();

            FilteredTransactions.Refresh();
        }

        private async void CloseStateSupportObj(TransactionObject transactionObject)
        {
            QuestionDialogView view = new()
            {
                DataContext = new QuestionDialogViewModel($"Вы действительно хотите закрыть \"{transactionObject}\"?{Environment.NewLine}" +
                $"Вы всегда сможете восстановить его в настройках")
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if ((bool)result == false)
                return;

            transactionObject.Close();
            DbContext.SaveChanges();

            RefreshTransactionObjectView(transactionObject);

            OnPropertyChanged("TotalMoney");
        }

        private async void RemoveStateSupportObj(TransactionObject transactionObject)
        {
            QuestionDialogView view = new()
            {
                DataContext = new QuestionDialogViewModel($"Вы действительно хотите удалить \"{transactionObject}\"?{Environment.NewLine}" +
                $"Ее нельзя будет восстановить в настройках")
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if ((bool)result == false)
                return;

            transactionObject.Delete();
            DbContext.SaveChanges();

            RefreshTransactionObjectView(transactionObject);

            OnPropertyChanged("TotalMoney");
        }

        private async void RemoveTransaction(Transaction transaction)
        {
            QuestionDialogView view = new()
            {
                DataContext = new QuestionDialogViewModel("Вы действительно хотите удалить эту транзакцию?")
            };

            var result = await DialogHost.Show(view, "RootDialog");
            if ((bool)result == false)
                return;

            transaction.RollBack();

            DbContext.Entry(transaction).State = EntityState.Deleted;
            DbContext.SaveChanges();
        }

        private void UpdateCard(PaymentType existingPaymentType)
        {
            if (!DialogService.OpenDialog(new CardDialogViewModel("Редактирование счета",
                                                                     DbContext.PaymentTypes
                                                                     .Where(pt => pt.TransactionObjectId != existingPaymentType.TransactionObjectId
                                                                               && (pt.State == StatesEnum.Active || pt.State == StatesEnum.Closed))
                                                                     .Select(pt => pt.TransactionObjectName)
                                                                     .ToHashSet(),
                                                                     existingPaymentType
                                                                     )))

                return;

            DbContext.SaveChanges();

            OnPropertyChanged("TotalMoney");
        }

        private void UpdateCategory(Category existingCategory)
        {
            if (!DialogService.OpenDialog(new CategoryDialogVM("Редактирование категории",
                                                       DbContext.Categories
                                                         .Where(ct => ct.TransactionObjectId != existingCategory.TransactionObjectId
                                                                   && (ct.State == StatesEnum.Active || ct.State == StatesEnum.Closed))
                                                         .AsEnumerable()
                                                         .Select(ct => (ct.TransactionObjectName, ct.CategoryType))
                                                         .ToHashSet(),
                                                         existingCategory)))
                return;

            DbContext.SaveChanges();
        }

        private void UpdateTransaction(Transaction existingTransaction)
        {
            if (existingTransaction == null)//TODO: off doubleClick trigger for ListView header
                return;

            Transaction backUpExTr = (Transaction)existingTransaction.Clone();

            if (!DialogService.OpenDialog(new TransactionDialogVM("Редактирование транзакции", PaymentTypes, Categories, existingTransaction)))
                return;

            if (backUpExTr.SourceFk != existingTransaction.SourceFk
             || backUpExTr.DestinationFk != existingTransaction.DestinationFk
             || backUpExTr.Price != existingTransaction.Price)
            {
                backUpExTr.RollBack();

                existingTransaction.Apply();
            }

            DbContext.SaveChanges();

            LoadTransactions();

            OnPropertyChanged("TotalMoney");
        }

        private bool FilterCategory(Category category) => category.State == StatesEnum.Active;
        private bool FilterPaymentType(PaymentType paymentType) => paymentType.State == StatesEnum.Active;
        private bool FilterTransaction(Transaction transaction) => transaction.DateTime > StartDate && transaction.DateTime <= EndDate;

        private void RefreshTransactionObjectView(TransactionObject transactionObject)
        {
            if (transactionObject is Category)
            {
                FilteredCategories.Refresh();
            }
            else if (transactionObject is PaymentType)
            {
                FilteredPaymentTypes.Refresh();
            }
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