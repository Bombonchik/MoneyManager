using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using MoneyManager.Abstractions;
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
        private TransactionTypeView selectedTransactionType;
        //private ObservableCollection<Category> currentCategories;

        public bool IsEditMode { get; private set; }
        private readonly IMessenger Messenger = WeakReferenceMessenger.Default;
        public TransactionDisplay NewTransactionDisplay { get; set; }
        private ObservableCollection<Category> IncomeCategories { get; set; }
        private ObservableCollection<Category> ExpenseCategories { get; set; }
        public ObservableCollection<Category> CurrentCategories { get; set; }

        public TimeSpan SelectedTime { get; set; }
        private Dictionary<int, Account> AccountLookup { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
        public Account CurrentAccount
        {
            set
            {
                if (IsSelectingSourceAccount)
                {
                    SelectedSourceAccount = value;
                    IsSelectingSourceAccount = false;
                    if (!IsTransfer && CurrentCategory is null)
                        SelectCategory();
                    else
                    {
                        IsSelectingDestinationAccount = true;
                        if (!IsTransfer)
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
        public ICommand SelectCategoryCommand =>
            new Command(() =>
            {
                SelectCategory();
            });
        private void SelectCategory()
        {

            IsAccountsCollectionViewVisible = false;
            IsCategoriesCollectionViewVisible = true;
        }
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
                IsSelectingDestinationAccount = true;
            });
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
        public ObservableCollection<TransactionTypeView> TransactionTypes { get; set; }
        public bool IsTransfer { get; set; }

        public ICommand ClosePageCommand =>
            new Command(() =>
            {
                var y = NewTransactionDisplay;

                (App.Current.MainPage as AppShell).ViewModel.CloseAddPage();
            });
        public ICommand SaveTransactionCommand =>
            new Command(async () =>
            {
                if (SelectedSourceAccount is null || NewTransactionDisplay.Transaction.Amount < 0
                || (CurrentCategory is null && (SelectedTransactionType.Type == TransactionType.Income || SelectedTransactionType.Type == TransactionType.Expense))
                || (SelectedDestinationAccount is null && SelectedTransactionType.Type == TransactionType.Transfer))
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

            });
        private void SaveTransaction()
        {
            NewTransactionDisplay.Transaction.SourceAccountId = SelectedSourceAccount.Id;

        }
        public TransactionManagementViewModel(TransactionDisplay existingTransactionDisplay = null)
        {
            Accounts = new();
            TransactionTypes = new();
            foreach (TransactionType transactionType in Enum.GetValues(typeof(TransactionType)))
                TransactionTypes.Add(new TransactionTypeView { Type = transactionType, IsSelected = false  });
            FillCategories();
            IsAccountsCollectionViewVisible = true;
            IsCategoriesCollectionViewVisible = true;
            IsSelectingSourceAccount = true;
            Messenger.Register<AccountAddedMessage>(this, OnAccountAdded);
            Messenger.Register<AccountDeletedMessage>(this, OnAccountDeleted);
            Messenger.Register<ResponseAccountsMessage>(this, HandleAccountResponse);
            Messenger.Send(new RequestAccountsMessage(this));
            if (existingTransactionDisplay is not null)
                InitializeEditMode(existingTransactionDisplay);
            else
                InitializeNormalMode();
        }

        private void OnAccountDeleted(object recipient, AccountDeletedMessage message)
        {
            if (message.Sender == this) return;
            Accounts.Remove(message.DeletedAccount);
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

        private void InitializeEditMode(TransactionDisplay existingTransactionDisplay)
        {
            IsEditMode = true;
            NewTransactionDisplay = existingTransactionDisplay;
            SelectedTransactionType = TransactionTypes.Where(x => x.Type == existingTransactionDisplay.Transaction.Type).FirstOrDefault();
        }
        private void InitializeNormalMode()
        {
            IsEditMode = false;
            NewTransactionDisplay = new TransactionDisplay { Transaction = new Transaction(), TransactionView = new TransactionView() };
            SelectedTransactionType = TransactionTypes.Where(x => x.Type == TransactionType.Expense).FirstOrDefault();
            NewTransactionDisplay.Transaction.DateTime = DateTime.Now;
            SelectedTime = NewTransactionDisplay.Transaction.DateTime.TimeOfDay;
        }

        private void FillCategories()
        {
            IncomeCategories = new ObservableCollection<Category>
            {
                // Income Categories
                new Category
                {
                    Id = 1,
                    Name = "Salary",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 2,
                    Name = "Business",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 3,
                    Name = "Investments",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 4,
                    Name = "Rental",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 5,
                    Name = "Dividends",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 6,
                    Name = "Pension",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 7,
                    Name = "Interest",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 8,
                    Name = "Freelance",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 9,
                    Name = "Sale of Assets",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 10,
                    Name = "Social Security",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 11,
                    Name = "Grant",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 12,
                    Name = "Lottery",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 13,
                    Name = "Tax Refund",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 14,
                    Name = "Royalties",
                    CategoryType = TransactionType.Income
                },
                new Category
                {
                    Id = 15,
                    Name = "Other",
                    CategoryType = TransactionType.Income
                }
            };
            ExpenseCategories = new ObservableCollection<Category>
            {
                new Category
                {
                    Id = 16,
                    Name = "Food",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 17,
                    Name = "Housing",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 18,
                    Name = "Transportation",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 19,
                    Name = "Eating Out",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 20,
                    Name = "Health",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 21,
                    Name = "Entertainment",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 22,
                    Name = "Sports",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 23,
                    Name = "Shopping",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 24,
                    Name = "Education",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 25,
                    Name = "Savings",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 26,
                    Name = "Investments",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 27,
                    Name = "Family",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 28,
                    Name = "Travel",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 29,
                    Name = "Pets",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 30,
                    Name = "Subscriptions",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 31,
                    Name = "Insurance",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 32,
                    Name = "Personal Care",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 33,
                    Name = "Debt Payments",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 34,
                    Name = "Communications",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 35,
                    Name = "Gifts",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 36,
                    Name = "Taxes",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 37,
                    Name = "Charity",
                    CategoryType = TransactionType.Expense
                },
                new Category
                {
                    Id = 38,
                    Name = "Other",
                    CategoryType = TransactionType.Expense
                }
            };
        }
    }
}
