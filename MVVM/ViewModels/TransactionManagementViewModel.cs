using CommunityToolkit.Mvvm.Messaging;
using MoneyManager.Abstractions;
using MoneyManager.DataTemplates;
using MoneyManager.Messages;
using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Transaction = MoneyManager.MVVM.Models.Transaction;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionManagementViewModel : BaseViewModel
    {
        private TransactionTypeView selectedTransactionType;
        private ObservableCollection<Category> currentCategories;

        public bool IsEditMode { get; private set; }
        private readonly IMessenger Messenger = WeakReferenceMessenger.Default;
        public TransactionDisplay NewTransactionDisplay { get; set; }
        private ObservableCollection<Category> IncomeCategories { get; set; } 
        private ObservableCollection<Category> ExpenseCategories { get; set; }
        public ObservableCollection<Category> CurrentCategories
        {
            get
            {
                if (SelectedTransactionType.Type == TransactionType.Income)
                    return IncomeCategories;
                if (SelectedTransactionType.Type == TransactionType.Expense)
                    return ExpenseCategories;
                return null;
            }
            set => currentCategories = value;
        }
        public TimeSpan SelectedTime { get; set; }
        private Dictionary<int, Account> AccountLookup { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
        public Account CurrentAccount { get; set; }
        public Account SelectedAccount { get; set; }
        public Account SelectedAccountFrom { get; set; }
        public Account SelectedAccountTo { get; set; }
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
            }
        }
        private Category SelectedIncomeCategory { get; set; }
        private Category SelectedExpenseCategory { get; set; }
        public bool IsAccountsCollectionViewVisible { get; set; }
        public bool IsCategoriesCollectionViewVisible { get; set; }
        public string AccountPlaceholder
        {
            get
            {
                if (SelectedTransactionType.Type == TransactionType.Income || SelectedTransactionType.Type == TransactionType.Expense)
                    return "Select an Account";
                return string.Empty;
            }
        }
        public string CategoryPlaceholder
        {
            get
            {
                if (AccountPlaceholder != string.Empty && SelectedAccount is not null)
                    return "Select a category";
                return string.Empty;
            }
        }
        public string AccountFromPlaceholder
        {
            get
            {
                if (SelectedTransactionType.Type == TransactionType.Transfer)
                    return "Select an Account";
                return string.Empty;
            }

        }
        public string AccountToPlaceholder
        {
            get
            {
                if (AccountFromPlaceholder != string.Empty && SelectedAccountFrom is not null)
                    return "Select a category";
                return string.Empty;
            }
        }
        public ICommand SelectAccountCommand =>
            new Command(() => 
            { 
            
            });
        public ICommand SelectCategoryCommand =>
            new Command(() =>
            {

            });
        public ICommand SelectAccountFromCommand =>
            new Command(() =>
            {

            });
        public ICommand SelectAccountToCommand =>
            new Command(() =>
            {

            });
        public TransactionTypeView SelectedTransactionType
        {
            get => selectedTransactionType; set
            {
                if (selectedTransactionType != value)
                {
                    OnNewItemSelected(selectedTransactionType, value);
                    selectedTransactionType = value;
                    TransferIndicator = selectedTransactionType.Type == TransactionType.Transfer;
                }
            }
        }
        public ObservableCollection<TransactionTypeView> TransactionTypes { get; set; }
        public bool IncomeIndicator { get; set; }
        public bool ExpenseIndicator { get; set; }
        public bool TransferIndicator { get; set; }

        public ICommand ClosePageCommand =>
            new Command(() =>
            {
                var y = NewTransactionDisplay;

                (App.Current.MainPage as AppShell).ViewModel.CloseAddPage();
            });
        public TransactionManagementViewModel(TransactionDisplay existingTransactionDisplay = null)
        {
            Accounts = new();
            TransactionTypes = new();
            foreach (TransactionType transactionType in Enum.GetValues(typeof(TransactionType)))
                TransactionTypes.Add(new TransactionTypeView { Type = transactionType, IsSelected = false  });
            FillCategories();
            //IsAccountsCollectionViewVisible = true;
            IsCategoriesCollectionViewVisible = true;
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
            SelectedTransactionType = new TransactionTypeView { Type = NewTransactionDisplay.Transaction.Type, IsSelected = true };
            //SelectedTransactionType = TransactionTypes.Where(x => x.Type == existingTransactionDisplay.Transaction.Type).FirstOrDefault();
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
