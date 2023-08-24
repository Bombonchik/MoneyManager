using Bogus;
using Bogus.DataSets;
using CommunityToolkit.Mvvm.Messaging;
using MoneyManager.Abstractions;
using MoneyManager.Constants;
using MoneyManager.DataTemplates;
using MoneyManager.Messages;
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
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AccountsViewModel : BaseViewModel
    {
        private readonly IMessenger Messenger = WeakReferenceMessenger.Default;

        public AccountDisplay CurrentAccountDisplay { get; set; } = new AccountDisplay();
        public ICommand AccountSelectedCommand { get; set; }

        public AccountDisplay SelectedAccountDisplay
        {
            get => selectedAccountDisplay; set
            {
                if (selectedAccountDisplay != value)
                {
                    OnNewItemSelected(selectedAccountDisplay.AccountView, value.AccountView);
                    selectedAccountDisplay = value;
                    Debug.WriteLine($"SelectedAccountDisplay: {selectedAccountDisplay.ToString()}");
                }
            }
        }
        private Dictionary<int, Account> AccountLookup { get; set; }
        private Dictionary<int, DeletedAccount> DeletedAccountLookup { get; set; }
        public CachedAccountsData CachedAccountsData { get; set; }
        private decimal CurrentTotalBalance { get; set; }
        public object SelectedItem { get; set; }
        private AccountDisplay selectedAccountDisplay = new AccountDisplay();

        public ObservableCollection<AccountDisplay> AccountDisplays { get; set; } = new ObservableCollection<AccountDisplay>();
        public AccountsViewModel() 
        {
            _ = InitializeAsync();
        }
        private async Task InitializeAsync(int accountsToGenerateCount = 0)
        {
            await ProcessAccounts();
            await ProcessDeletedAccounts();
            if (accountsToGenerateCount > 0)
                FillDataAsync(AccountDisplays.Count, accountsToGenerateCount);
            Debug.WriteLine(AccountDisplays.MyToString());
            CachedAccountsData = await App.CachedAccountsDataService.GetSafelyLastCashedAccountsDataAsync(RecalculateTotalBalance);
            Debug.WriteLine(CachedAccountsData.ToString());
            Messenger.Register<RequestAccountsMessage>(this, HandleAccountRequest);
        
        }

        private void HandleAccountRequest(object recipient, RequestAccountsMessage message)
        {
            Messenger.Send(new ResponseAccountsMessage(AccountLookup, message.Sender));
        }

        private async Task ProcessAccounts()
        {
            AccountDisplays = await GetAccountDisplaysAsync();
            AccountLookup = new Dictionary<int, Account>();
            foreach (var accountDisplay in AccountDisplays)
                AccountLookup[accountDisplay.Account.Id] = accountDisplay.Account;
        }
        private async Task ProcessDeletedAccounts()
        {
            var deletedAccounts = await GetDeletedAccountsAsync();
            DeletedAccountLookup = new Dictionary<int, DeletedAccount>();
            foreach (var deletedAccount in deletedAccounts)
                DeletedAccountLookup[deletedAccount.DeletedAccountId] = deletedAccount;
        }

        private decimal RecalculateTotalBalance()
        {
            decimal balance = 0;
            foreach (var accountDisplay in AccountDisplays) 
            { 
                balance += accountDisplay.Account.Balance;
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
                if (SelectedAccountDisplay.AccountView is null)
                {
                    await Application.Current.MainPage.DisplayAlert("You should select the account you want to delete", "", "OK");
                    return;
                }
                bool response = await Application.Current.MainPage.DisplayAlert("Are you sure you want to delete this account?",
                    "All recurring transactions associated with this account will also be deleted.", "Yes", "No");
                if (response)
                {
                    await ChangeTotalBalanceAsync(-SelectedAccountDisplay.Account.Balance);
                    await DeleteAccountAsync(SelectedAccountDisplay.Account);
                    await DeleteAccountViewAsync(SelectedAccountDisplay.AccountView);
                    Debug.WriteLine($"Remove from AccountDisplays: {SelectedAccountDisplay}");
                    AccountDisplays.Remove(SelectedAccountDisplay);
                    Debug.WriteLine($"Current : {AccountDisplays.MyToString()}");
                    Debug.WriteLine($"Remove from Accounts: {SelectedAccountDisplay.Account}");
                    Debug.WriteLine($"Remove from AccountViews: {SelectedAccountDisplay.AccountView}");
                    selectedAccountDisplay = new AccountDisplay();
                    
                    //List<DeletedAccount> deletedAccounts = await App.DeletedAccountRepo.GetItemsAsync();
                    //Debug.WriteLine($"DeletedAccounts : {deletedAccounts.MyToString()}");
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
                    await SaveAccountViewAsync(newAccountDisplay.AccountView);
                    await SaveAccountAsync(newAccountDisplay.Account);
                    Debug.WriteLine(AccountDisplays.MyToString());
                    SelectedAccountDisplay = newAccountDisplay;
                    await ChangeTotalBalanceAsync(newAccountDisplay.Account.Balance);
                };
                await Shell.Current.Navigation.PushAsync(accountManagmentPage);
            }); 
        public ICommand OpenEditAccountPageCommand =>
            new Command(async () =>
            {
                if (SelectedAccountDisplay.AccountView is null)
                {
                    await Application.Current.MainPage.DisplayAlert("You should select the account you want to edit","", "OK");
                    return;
                }
                var currentSelectedAccountBalance = SelectedAccountDisplay.Account.Balance;
                var accountManagmentPage = new AccountManagementView(SelectedAccountDisplay);
                var viewModel = ((AccountManagementViewModel)accountManagmentPage.BindingContext);
                viewModel.AccountSavedCallback = async (newAccountDisplay) =>
                {
                    await UpdateAccountAsync(newAccountDisplay.Account);
                    await ChangeTotalBalanceAsync(newAccountDisplay.Account.Balance - currentSelectedAccountBalance);
                    SelectedAccountDisplay = new AccountDisplay { Account = newAccountDisplay.Account, AccountView = newAccountDisplay.AccountView };
                };
                await Shell.Current.Navigation.PushAsync(accountManagmentPage);
            });


        #region DatabaseInteraction
        private async Task SaveAccountAsync(Account account)
        {
            await App.AccountsRepo.SaveItemAsync(account);
            AccountLookup.Add(account.Id, account);
            Messenger.Send(new AccountAddedMessage(account, this));
        }
        private async Task UpdateAccountAsync(Account account)
        {
            await App.AccountsRepo.SaveItemAsync(account);
            AccountLookup[account.Id] = account;
            Messenger.Send(new AccountUpdatedMessage(account, this));
        }
        private async Task SaveAccountViewAsync(AccountView accountView)
        {
            await App.AccountViewsRepo.SaveItemAsync(accountView);
        }
        private async Task DeleteAccountAsync(Account account)
        {
            await App.AccountsRepo.DeleteItemAsync(account);
            var deletedAccount = new DeletedAccount { DeletedAccountId = account.Id, Name = account.Name };
            await App.DeletedAccountRepo.SaveItemAsync(deletedAccount);
            DeletedAccountLookup.Add(deletedAccount.DeletedAccountId, deletedAccount);
            AccountLookup.Remove(deletedAccount.DeletedAccountId);
            Messenger.Send(new AccountDeletedMessage(account, this));
        }
        public async Task<List<Account>> GetAccountsAsync()
        {
            return await App.AccountsRepo.GetItemsAsync();
        }
        public async Task<List<DeletedAccount>> GetDeletedAccountsAsync()
        {
            return await App.DeletedAccountRepo.GetItemsAsync();
        }
        private async Task DeleteAccountViewAsync(AccountView accountView)
        {
            await App.AccountViewsRepo.DeleteItemAsync(accountView);
        }

        public static async Task<List<AccountView>> GetAccountsAccountViewsAsync()
        {
            return await App.AccountViewsRepo.GetItemsAsync();
        }
        
        private async Task<ObservableCollection<AccountDisplay>> GetAccountDisplaysAsync()
        {
            var accounts = await GetAccountsAsync();
            var accountViews = await GetAccountsAccountViewsAsync();  // Fetch all account views from the database
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
        private async void FillDataAsync(int currentAccountNumber, int accountsToGenerateCount)
        {
            decimal totalBalanceChange = 0;
            for (int i = 0; i < accountsToGenerateCount; i++)
            {
                CurrentAccountDisplay = new AccountDisplay(); 
                currentAccountNumber++;
                GenerateNewAccount(currentAccountNumber);
                GenerateNewAccountView(currentAccountNumber);
                await SaveAccountAsync(CurrentAccountDisplay.Account);
                await SaveAccountViewAsync(CurrentAccountDisplay.AccountView);
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

    #region CollectionExtensions
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
    #endregion
}
