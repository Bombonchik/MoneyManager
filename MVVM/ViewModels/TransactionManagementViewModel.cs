using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using MoneyManager.Abstractions;
using MoneyManager.Constants;
using MoneyManager.DataTemplates;
using MoneyManager.Messages;
using MoneyManager.MVVM.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Transaction = MoneyManager.MVVM.Models.Transaction;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionManagementViewModel : BaseViewModel
    {
        #region Properties
        private Account currentAccount = null;
        private TransactionTypeView selectedTransactionType;
        private bool IsTransactionCreating { get; set; }
        public bool IsEditMode { get; private set; }
        private readonly IMessenger Messenger = WeakReferenceMessenger.Default;
        public TransactionDisplay NewTransactionDisplay { get; set; }
        public TransactionDisplay OldTransactionDisplay { get; set; }
        private ObservableCollection<Category> IncomeCategories { get; set; }
        private ObservableCollection<Category> ExpenseCategories { get; set; }
        public ObservableCollection<Category> CurrentCategories { get; set; }

        public TimeSpan SelectedTime { get; set; }
        private Dictionary<int, Account> AccountLookup { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
        public ObservableCollection<TransactionTypeView> TransactionTypes { get; set; }
        public bool IsTransfer { get; set; }
        public Account CurrentAccount
        {
            get { return currentAccount; }
            set
            {
                if (IsSelectingSourceAccount)
                {
                    SelectedSourceAccount = value;
                    IsSelectingSourceAccount = false;
                    if (IsTransfer)
                    {
                        if (SelectedDestinationAccount is null)
                            IsSelectingDestinationAccount = true;
                        else
                            IsAccountsCollectionViewVisible = false;
                    }
                    else
                    {
                        if (CurrentCategory is null)
                            SelectCategory();
                        else
                            IsAccountsCollectionViewVisible = false;
                    }
                    return;
                }
                if (IsSelectingDestinationAccount)
                {
                    SelectedDestinationAccount = value;
                    IsSelectingDestinationAccount = false;
                    if (SelectedDestinationAccount is not null && SelectedTransactionType is not null)
                        IsAccountsCollectionViewVisible = false;
                    return;
                }
            }
        }
        public TransactionTypeView SelectedTransactionType
        {
            get => selectedTransactionType;
            set
            {
                if (value is null) return;
                if (selectedTransactionType != value)
                {
                    OnNewItemSelected(selectedTransactionType, value);
                    selectedTransactionType = value;
                    IsTransfer = selectedTransactionType.Type == TransactionType.Transfer;
                    UpdateUIForTransactionType(selectedTransactionType.Type);
                }
            }
        }
        public Account SelectedSourceAccount { get; set; }
        public Account SelectedDestinationAccount { get; set; }
        private const string CategorySelectionText = "Select a Category";
        private const string AccountSelectionText = "Select an Account";
        public string SourceAccountLabelText => IsTransfer ? "From" : "Account";
        public Category CurrentCategory
        {
            get
            {
                if (SelectedTransactionType.Type == TransactionType.Income)
                    return SelectedIncomeCategory;
                if (selectedTransactionType.Type == TransactionType.Expense)
                    return SelectedExpenseCategory;
                return null;
            }
            set
            {
                if (SelectedTransactionType.Type == TransactionType.Income)
                    SelectedIncomeCategory = value;
                if (selectedTransactionType.Type == TransactionType.Expense)
                    SelectedExpenseCategory = value;
                IsCategoriesCollectionViewVisible = false;
            }
        }
        private Category SelectedIncomeCategory { get; set; }
        private Category SelectedExpenseCategory { get; set; }
        public bool IsAccountsCollectionViewVisible { get; set; }
        public bool IsCategoriesCollectionViewVisible { get; set; }
        public bool IsSelectingSourceAccount { get; set; }
        public bool IsSelectingDestinationAccount { get; set; }

        public string CategoryPlaceholder
        {
            get
            {
                if (SelectedSourceAccount is not null)
                    return CategorySelectionText;
                return string.Empty;
            }
        }
        public string SourceAccountPlaceholder
        {
            get
            {
                return AccountSelectionText;
            }
        }
        public string DestinationAccountPlaceholder
        {
            get
            {
                if (SelectedSourceAccount is not null)
                    return AccountSelectionText;
                return string.Empty;
            }
        }
        #endregion
        #region Commands
        public ICommand SelectCategoryCommand =>
            new Command(() =>
            {
                SelectCategory();
            });
        public ICommand SelectSourceAccountCommand =>
            new Command(() =>
            {
                IsCategoriesCollectionViewVisible = false;
                IsAccountsCollectionViewVisible = true;
                IsSelectingSourceAccount = true;
            });
        public ICommand SelectDestinationAccountCommand =>
            new Command(() =>
            {
                IsCategoriesCollectionViewVisible = false;
                IsAccountsCollectionViewVisible = true;
                IsSelectingSourceAccount = false;
                IsSelectingDestinationAccount = true;
            });
        public ICommand SaveTransactionCommand =>
            new Command(async () =>
            {
                if (SelectedSourceAccount is null || NewTransactionDisplay.Transaction.Amount < 0
                || (CurrentCategory is null && (SelectedTransactionType.Type == TransactionType.Income || SelectedTransactionType.Type == TransactionType.Expense))
                || (SelectedDestinationAccount is null && SelectedTransactionType.Type == TransactionType.Transfer)
                || !AccountLookup.ContainsKey(SelectedSourceAccount.Id)
                || (SelectedDestinationAccount is not null && !AccountLookup.ContainsKey(SelectedDestinationAccount.Id)))
                {
                    await Application.Current.MainPage.DisplayAlert($"Unable to {(IsEditMode ? "save" : "create")} a transaction",
                            "Select account(s) and enter a nonnegative amount", "OK");
                    return;
                }
                if ((SelectedTransactionType.Type == TransactionType.Expense || SelectedTransactionType.Type == TransactionType.Transfer)
                && SelectedSourceAccount.Balance < NewTransactionDisplay.Transaction.Amount)
                {
                    await Application.Current.MainPage.DisplayAlert($"Unable to {(IsEditMode ? "save" : "create")} a transaction",
                             $"{(SelectedTransactionType.Type == TransactionType.Expense ? "Expense" : "Transfer")} amount is less than the balance of {SelectedSourceAccount.Name}", "OK");
                    return;
                }
                FillInTransactionData();
                await ProcessTransaction();
                ClosePage();
            });
        public ICommand CancelTransactionCommand =>
            new Command(() =>
            {
                ClosePage();
            });
        #endregion
        #region Methods
        private void SelectCategory()
        {

            IsAccountsCollectionViewVisible = false;
            IsCategoriesCollectionViewVisible = true;
        }
        private void UpdateUIForTransactionType(TransactionType type)
        {
            if (type == TransactionType.Income || type == TransactionType.Expense)
            {
                if (SelectedSourceAccount is not null && CurrentCategory is not null)
                {
                    IsCategoriesCollectionViewVisible = false;
                    IsAccountsCollectionViewVisible = false;
                    return;
                }
                if (SelectedSourceAccount is not null && CurrentCategory is null)
                {
                    IsAccountsCollectionViewVisible = false;
                    IsCategoriesCollectionViewVisible = true;
                    IsSelectingSourceAccount = true;
                }
                CurrentCategories = (type == TransactionType.Income) ? IncomeCategories : ExpenseCategories;
                return;
            }
            if (type == TransactionType.Transfer)
            {
                IsCategoriesCollectionViewVisible = false;
                if (SelectedSourceAccount is not null && SelectedDestinationAccount is not null)
                {
                    IsAccountsCollectionViewVisible = false;
                    return;
                }
                if (SelectedSourceAccount is not null && SelectedDestinationAccount is null)
                {
                    IsSelectingDestinationAccount = true;
                    IsSelectingSourceAccount = false;
                    IsAccountsCollectionViewVisible = true;
                }
                return;
            }
        }

        private void ClosePage()
        {
            (App.Current.MainPage as AppShell).ViewModel.CloseAddPage();
            ResetPageData();
            IsTransactionCreating = false;
        }
        
        private async Task ProcessTransaction()
        {
            if (IsEditMode)
                await UpdateTransactionAsync(OldTransactionDisplay.Transaction, NewTransactionDisplay.Transaction);
            else
                await SaveTransactionAsync(NewTransactionDisplay.Transaction);
        }

        private async Task SaveTransactionAsync(Transaction transaction)
        {
            await App.TransactionsRepo.SaveItemAsync(transaction);
            Messenger.Send(new TransactionAddedMessage(transaction, this));
        }

        private async Task UpdateTransactionAsync(Transaction oldTransaction, Transaction newTransaction)
        {
            await App.TransactionsRepo.SaveItemAsync(newTransaction);
            Messenger.Send(new TransactionUpdatedMessage(oldTransaction, newTransaction, this));
        }

        private void FillInTransactionData()
        {
            NewTransactionDisplay.Transaction.SourceAccountId = SelectedSourceAccount.Id;
            NewTransactionDisplay.Transaction.DateTime = NewTransactionDisplay.Transaction.DateTime.Date + SelectedTime;
            NewTransactionDisplay.Transaction.Type = SelectedTransactionType.Type;
            if (SelectedTransactionType.Type == TransactionType.Income || SelectedTransactionType.Type == TransactionType.Expense)
                NewTransactionDisplay.Transaction.CategoryId = CurrentCategory.Id;
            else if (SelectedTransactionType.Type == TransactionType.Transfer)
                NewTransactionDisplay.Transaction.DestinationAccountId = SelectedDestinationAccount.Id;

        }
        private void ResetPageData()
        {
            SelectedSourceAccount = null;
            SelectedDestinationAccount = null;
            NewTransactionDisplay = null;
            OldTransactionDisplay = null;
            SelectedIncomeCategory = null;
            SelectedExpenseCategory = null;
            IsSelectingSourceAccount = false;
            IsSelectingDestinationAccount = false;
        }
        private void GetCategories()
        {
            IncomeCategories = CategoryConstants.IncomeCategories;
            ExpenseCategories = CategoryConstants.ExpenseCategories;
        }
        #endregion
        public TransactionManagementViewModel(TransactionDisplay existingTransactionDisplay = null)
        {
            Accounts = new();
            TransactionTypes = new();
            foreach (TransactionType transactionType in Enum.GetValues(typeof(TransactionType)))
                TransactionTypes.Add(new TransactionTypeView { Type = transactionType, IsSelected = false });
            GetCategories();
            Messenger.Register<AccountAddedMessage>(this, OnAccountAdded);
            Messenger.Register<AccountDeletedMessage>(this, OnAccountDeleted);
            Messenger.Register<ResponseAccountsMessage>(this, HandleAccountResponse);
            Messenger.Send(new RequestAccountsMessage(this));
            InitializeTransaction(existingTransactionDisplay);
            IsAccountsCollectionViewVisible = true;
            IsCategoriesCollectionViewVisible = true;
        }
        #region Event Handlers
        private void OnAccountDeleted(object recipient, AccountDeletedMessage message)
        {
            if (message.Sender == this) return;
            Accounts.Remove(message.Account);
        }

        private void HandleAccountResponse(object recipient, ResponseAccountsMessage message)
        {
            if (message.Sender != this) return;
            AccountLookup = message.Accounts;
            Accounts = new ObservableCollection<Account>(AccountLookup.Values);
        }

        private void OnAccountAdded(object recipient, AccountAddedMessage message)
        {
            if (message.Sender == this) return;
            Accounts.Add(AccountLookup[message.NewAccount.Id]);
        }
        #endregion

        #region Initializing
        public void InitializeTransaction(TransactionDisplay existingTransactionDisplay = null)
        {
            IsTransactionCreating = true;
            if (existingTransactionDisplay is not null)
                InitializeEditMode(existingTransactionDisplay);
            else
                InitializeNormalMode();
            IsAccountsCollectionViewVisible = true;
            IsCategoriesCollectionViewVisible = false;
            IsSelectingSourceAccount = true;
        }
        private void InitializeEditMode(TransactionDisplay existingTransactionDisplay)
        {
            IsEditMode = true;
            OldTransactionDisplay = existingTransactionDisplay;
            NewTransactionDisplay = new TransactionDisplay
            {
                Transaction = OldTransactionDisplay.Transaction.Clone(),
                TransactionView = OldTransactionDisplay.TransactionView.Clone()
            };
            SelectedTransactionType = TransactionTypes.Where(x => x.Type == existingTransactionDisplay.Transaction.Type).FirstOrDefault();
            SelectedSourceAccount = AccountLookup[NewTransactionDisplay.Transaction.SourceAccountId];
            if (NewTransactionDisplay.Transaction.DestinationAccountId is not null)
                SelectedDestinationAccount = AccountLookup[NewTransactionDisplay.Transaction.DestinationAccountId.Value];
            if (NewTransactionDisplay.Transaction.CategoryId is not null)
            {
                var categoryId = NewTransactionDisplay.Transaction.CategoryId;
                if (selectedTransactionType.Type == TransactionType.Income)
                    CurrentCategory = IncomeCategories.Where(x => x.Id == categoryId).FirstOrDefault();
                else
                    CurrentCategory = ExpenseCategories.Where(x => x.Id == categoryId).FirstOrDefault();
            }
            SelectedTime = NewTransactionDisplay.Transaction.DateTime.TimeOfDay;
        }
        private void InitializeNormalMode()
        {
            IsEditMode = false;
            NewTransactionDisplay = new TransactionDisplay { Transaction = new Transaction(), TransactionView = new TransactionView() };
            SelectedTransactionType = TransactionTypes.Where(x => x.Type == TransactionType.Expense).FirstOrDefault();
            var dateTime = DateTime.Now;
            NewTransactionDisplay.Transaction.DateTime = dateTime.Date;
            SelectedTime = DateTime.Now.TimeOfDay;
        }
        public void OnAppearing()
        {
            if (!IsTransactionCreating)
            {
                InitializeTransaction();
            }
        }
        #endregion

    }
}
