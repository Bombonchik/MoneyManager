using Bogus;
using Bogus.DataSets;
using MoneyManager.Constants;
using MoneyManager.DataTemplates;
using MoneyManager.MVVM.Models;
using MoneyManager.MVVM.Views;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AccountsViewModel
    {
        public AccountDisplay CurrentAccountDisplay { get; set; } = new AccountDisplay();
        public ICommand AccountSelectedCommand { get; set; }

        public AccountDisplay SelectedAccountDisplay
        {
            get => selectedAccountDisplay; set
            {
                if (selectedAccountDisplay != value)
                {
                    OnNewAccountSelected(selectedAccountDisplay, value);
                    selectedAccountDisplay = value;
                    Debug.WriteLine($"SelectedAccountDisplay: {selectedAccountDisplay.ToString()}");
                }
            }
        }

        public CachedAccountsData CachedAccountsData { get; set; }
        private decimal CurrentTotalBalance { get; set; }
        public object SelectedItem { get; set; }
        private List<Account> Accounts { get; set; }
        private List<AccountView> AccountViews { get; set; }
        private AccountDisplay selectedAccountDisplay;


        public ObservableCollection<AccountDisplay> AccountDisplays { get; set; } = new ObservableCollection<AccountDisplay>();

        public AccountsViewModel() 
        {
            _ = InitializeAsync();
        }
        private async Task InitializeAsync(bool generateNewData = false)
        {
            AccountDisplays = await GetAccountDisplaysAsync();
            Debug.WriteLine(AccountDisplays.MyToString());
            CachedAccountsData = await App.CachedAccountsDataService.GetSafelyLastCashedAccountsDataAsync(RecalculateTotalBalance);
            Debug.WriteLine(CachedAccountsData.ToString());
            if (generateNewData)
                FillDataAsync(AccountDisplays.Count);
            //CurrentTotalBalance = RecalculateAllBalances
        }
        private decimal RecalculateTotalBalance()
        {
            decimal balance = 0;
            foreach (var account in Accounts) 
            { 
                balance += account.Balance;
            }
            return balance;
        }
        private async Task ChangeTotalBalanceAsync(decimal totalBalanceChange)
        {
            Debug.WriteLine($"ChangeTotalBalanceAsync {CachedAccountsData.TotalBalance} on {totalBalanceChange}");
            CachedAccountsData.TotalBalance += totalBalanceChange;
            await App.CachedAccountsDataRepo.SaveItemAsync(CachedAccountsData);
        }
        public ICommand DeleteAccountCommand =>
            new Command(async () =>
            {
                if (SelectedAccountDisplay is null)
                {
                    await Application.Current.MainPage.DisplayAlert("You should select the account you want to delete", "", "OK");
                    return;
                }
                bool response = await Application.Current.MainPage.DisplayAlert("Are you sure you want to delete this account?",
                    "All recurring transactions associated with this account will also be deleted.", "Yes", "No");
                if (response)
                {
                    await ChangeTotalBalanceAsync(-SelectedAccountDisplay.Account.Balance);
                    await App.RecurringTransactionsService.DeleteRecurringTransactionsAssociatedWithAccountAsync(SelectedAccountDisplay.Account);
                    await App.DeletedAccountRepo.SaveItemAsync(new DeletedAccount
                    { DeletedAccountId = SelectedAccountDisplay.Account.Id, Name = SelectedAccountDisplay.Account.Name });
                    await App.AccountsRepo.DeleteItemAsync(SelectedAccountDisplay.Account);
                    await App.AccountViewsRepo.DeleteItemAsync(SelectedAccountDisplay.AccountView);
                    Debug.WriteLine($"Remove from AccountDisplays: {SelectedAccountDisplay}");
                    AccountDisplays.Remove(SelectedAccountDisplay);
                    Debug.WriteLine($"Current : {AccountDisplays.MyToString()}");
                    Debug.WriteLine($"Remove from Accounts: {SelectedAccountDisplay.Account}");
                    Accounts.Remove(SelectedAccountDisplay.Account);
                    Debug.WriteLine($"Current : {Accounts.MyToString()}");
                    Debug.WriteLine($"Remove from AccountViews: {SelectedAccountDisplay.AccountView}");
                    AccountViews.Remove(SelectedAccountDisplay.AccountView);
                    Debug.WriteLine($"Current : {AccountViews.MyToString()}");
                    selectedAccountDisplay = null;
                    List<DeletedAccount> deletedAccounts = await App.DeletedAccountRepo.GetItemsAsync();
                    Debug.WriteLine($"DeletedAccounts : {deletedAccounts.MyToString()}");
                }
            });
        public ICommand OpenAddNewAccountPageCommand =>
            new Command(async () =>
            {
                var accountManagmentPage = new AccountManagementView();
                var viewModel = ((AccountManagementViewModel)accountManagmentPage.BindingContext);
                viewModel.AccountSavedCallback = async (newAccountDisplay) =>
                {
                    AccountDisplays.Add(newAccountDisplay);
                    Debug.WriteLine(AccountDisplays.MyToString());
                    SelectedAccountDisplay = newAccountDisplay;
                    await ChangeTotalBalanceAsync(newAccountDisplay.Account.Balance);
                };
                await Shell.Current.Navigation.PushAsync(accountManagmentPage);
            }); 
        public ICommand OpenEditAccountPageCommand =>
            new Command(async () =>
            {
                if (SelectedAccountDisplay is null)
                {
                    await Application.Current.MainPage.DisplayAlert("You should select the account you want to edit","", "OK");
                    return;
                }
                var currentSelectedAccountBalance = SelectedAccountDisplay.Account.Balance;
                var accountManagmentPage = new AccountManagementView(SelectedAccountDisplay);
                var viewModel = ((AccountManagementViewModel)accountManagmentPage.BindingContext);
                viewModel.AccountSavedCallback = async (newAccountDisplay) =>
                {
                    await ChangeTotalBalanceAsync(newAccountDisplay.Account.Balance - currentSelectedAccountBalance);
                    SelectedAccountDisplay = new AccountDisplay { Account = newAccountDisplay.Account, AccountView = newAccountDisplay.AccountView };
                };
                await Shell.Current.Navigation.PushAsync(accountManagmentPage);
            });
        private void OnNewAccountSelected(AccountDisplay previousSelectedAccount, AccountDisplay currentSelectedAccount)
        {
            if (previousSelectedAccount is not null)
                previousSelectedAccount.AccountView.IsSelected = false;
            currentSelectedAccount.AccountView.IsSelected = true;
        }

        #region DatabaseInteraction
        private async void DeleteAccountAsync()
        {
            await App.AccountsRepo.DeleteItemAsync(CurrentAccountDisplay.Account);
        }
        public static async Task<List<Account>> GetAccountsAsync()
        {
            return await App.AccountsRepo.GetItemsAsync();
        }
        private async void DeleteAccountViewAsync()
        {
            await App.AccountViewsRepo.DeleteItemAsync(CurrentAccountDisplay.AccountView);
        }

        public static async Task<List<AccountView>> GetAccountsAccountViewsAsync()
        {
            return await App.AccountViewsRepo.GetItemsAsync();
        }
        
        private async Task<ObservableCollection<AccountDisplay>> GetAccountDisplaysAsync()
        {
            var accounts = await GetAccountsAsync();  // Fetch all accounts from the database
            var accountViews = await GetAccountsAccountViewsAsync();  // Fetch all account views from the database
            Accounts = accounts;
            AccountViews = accountViews;
            // Create a dictionary for AccountView objects indexed by AccountId
            Dictionary<int, AccountView>  accountViewDict = accountViews.ToDictionary(av => av.AccountId);
            
            var accountDisplays = new ObservableCollection<AccountDisplay>();

            foreach (var account in accounts)
            {
                if (accountViewDict.TryGetValue(account.Id, out var accountView))
                {
                    accountDisplays.Add(new AccountDisplay
                    {
                        Account = account,
                        AccountView = accountView
                    });
                }
            }

            return accountDisplays;
        }
        #endregion
        #region DataGeneration
        private async void FillDataAsync(int currentAccountNumber)
        {
            decimal totalBalanceChange = 0;
            for (int i = 0; i < DisplayConstants.IconGlyphs.Count; i++)
            {
                CurrentAccountDisplay = new AccountDisplay(); 
                currentAccountNumber++;
                GenerateNewAccount(currentAccountNumber);
                GenerateNewAccountView(currentAccountNumber);
                await App.AccountsRepo.SaveItemAsync(CurrentAccountDisplay.Account);
                await App.AccountViewsRepo.SaveItemAsync(CurrentAccountDisplay.AccountView);
                await Console.Out.WriteLineAsync(App.AccountsRepo.StatusMessage);
                await Console.Out.WriteLineAsync(App.AccountViewsRepo.StatusMessage);
                AccountDisplays.Add(CurrentAccountDisplay);
                totalBalanceChange += CurrentAccountDisplay.Account.Balance;
                Debug.WriteLine(CurrentAccountDisplay.ToString());
            }
            await ChangeTotalBalanceAsync(totalBalanceChange);
        }
        private void GenerateNewAccount(int currentAccountNumber)
        {
            CurrentAccountDisplay.Account = new Faker<Account>()
                .RuleFor(a => a.Name, f =>
                {
                    var name = f.Company.CompanyName();
                    return name.Length <= 16 ? name : name.Substring(0, 16);
                })
                .RuleFor(a => a.Balance, f => f.Finance.Amount())
                .RuleFor(a => a.Identifier, f => f.Random.AlphaNumeric(16))
                .RuleFor(a => a.Type, f => f.PickRandom(new[] { "Cash", "Card", "Crypto", "Stocks", "Bank Account", "Credit Card", "Saving Account" }))
                .RuleFor(a => a.AccountViewId, f => currentAccountNumber)
                .Generate();
        }
        private void GenerateNewAccountView(int currentAccountNumber)
        {
            var rand = new Random();
            int index = rand.Next(DisplayConstants.IconGlyphs.Count);

            CurrentAccountDisplay.AccountView = new AccountView
            {
                AccountId = currentAccountNumber,
                BackgroundColor = App.ColorService.GetColorFromGradient(DisplayConstants.AccountBackgroundColorRange).ToHex(),
                Icon = DisplayConstants.IconGlyphs[index]
            };
        }
        #endregion
    }

    public static class ObservableCollectionExtensions
    {
        public static string MyToString<T>(this ObservableCollection<T> collection)
        {
            if (collection == null)
                return "null";

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < collection.Count; i++)
            {
                sb.Append(collection[i].ToString());
                if (i < collection.Count - 1)
                    sb.Append(", \n");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
    public static class List
    {
        public static string MyToString<T>(this List<T> collection)
        {
            if (collection == null)
                return "null";

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < collection.Count; i++)
            {
                sb.Append(collection[i].ToString());
                if (i < collection.Count - 1)
                    sb.Append(", \n");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
