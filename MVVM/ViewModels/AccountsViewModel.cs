using Bogus;
using Bogus.DataSets;
using MoneyManager.Constants;
using MoneyManager.DataTemplates;
using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
                    
                }
            }
        }

        public CachedAccountsData CashedAccountsData { get; set; }
        public object SelectedItem { get; set; }
        private List<Account> Accounts { get; set; }
        private List<AccountView> AccountViews { get; set; }
        private AccountDisplay selectedAccountDisplay;
        private static readonly (Color, Color) AccountBackgroundColorRange =  (Color.FromArgb("#49B6E3"), Color.FromArgb("#0208B4"));
        Color originalLightColor { get; set; }
        Color originalDarkColor { get; set; }
        const string LightStrokeColorName = "LightStrokeColor";
        const string DarkStrokeColorName = "DarkStrokeColor";


        public ObservableCollection<AccountDisplay> AccountDisplays { get; set; } = new ObservableCollection<AccountDisplay>();

        public AccountsViewModel() 
        {
            _ = InitializeAsync();
        }
        private async Task InitializeAsync(bool generateNewData = false)
        {
            AccountDisplays = await GetAccountDisplaysAsync();
            if (generateNewData)
                FillDataAsync(AccountDisplays.Count);
            CashedAccountsData = await GetLastCashedAccountsDataAsync();
            CashedAccountsData.TotalBalance = RecalculateAllBalances();
            await App.CashedAccountsDataRepo.SaveItemAsync(CashedAccountsData);

        }
        private decimal RecalculateAllBalances()
        {
            decimal balance = 0;
            foreach (var account in Accounts) 
            { 
                balance += account.Balance;
            }
            return balance;
        }
        public ICommand DeleteAccountCommand =>
            new Command(() =>
            {

            });
        private void OnNewAccountSelected(AccountDisplay previousSelectedAccount, AccountDisplay currentSelectedAccaunt)
        {
            if (previousSelectedAccount is not null)
                previousSelectedAccount.AccountView.IsSelected = false;
            currentSelectedAccaunt.AccountView.IsSelected = true;
        }

        #region DatabaseInteraction
        private async void DeleteAccountAsync()
        {
            await App.AccountsRepo.DeleteItemAsync(CurrentAccountDisplay.Account);
        }
        private async Task<List<Account>> GetAccountsAsync()
        {
            return await App.AccountsRepo.GetItemsAsync();
        }
        private async void DeleteAccountViewAsync()
        {
            await App.AccountViewsRepo.DeleteItemAsync(CurrentAccountDisplay.AccountView);
        }

        private async Task<List<AccountView>> GetAccountsAccountViewsAsync()
        {
            return await App.AccountViewsRepo.GetItemsAsync();
        }
        private async Task<List<CachedAccountsData>> GetAllCashedAccountsDataAsync()
        {
            return await App.CashedAccountsDataRepo.GetItemsAsync();
        }
        private async Task<CachedAccountsData> GetLastCashedAccountsDataAsync()
        {
            return await App.CashedAccountsDataRepo.GetLastItemAsync();
        }
        public async Task<ObservableCollection<AccountDisplay>> GetAccountDisplaysAsync()
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
                Debug.WriteLine(CurrentAccountDisplay.ToString());
            }
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
                .RuleFor(a => a.Type, f => f.PickRandom(new[] { "Cash", "Card", "Crypto", "Stocks" }))
                .RuleFor(a => a.AccoutViewId, f => currentAccountNumber)
                .Generate();
        }
        private void GenerateNewAccountView(int currentAccountNumber)
        {
            var rand = new Random();
            int index = rand.Next(DisplayConstants.IconGlyphs.Count);

            CurrentAccountDisplay.AccountView = new AccountView
            {
                AccountId = currentAccountNumber,
                BackgroundColor = GetColorFromGradient(AccountBackgroundColorRange).ToHex(),
                Icon = DisplayConstants.IconGlyphs[index]
            };
        }
        #endregion
        #region ColorMethods
        public Color GetColorFromGradient((Color, Color) colorRange)
        {
            Random rnd = new Random();

            // Define the start and end colors of your gradient
            Color startColor = colorRange.Item1;
            Color endColor = colorRange.Item2;

            // Generate a random position in the gradient
            double position = rnd.NextDouble();

            // Calculate the gradient color
            double r = startColor.Red + position * (endColor.Red - startColor.Red);
            double g = startColor.Green + position * (endColor.Green - startColor.Green);
            double b = startColor.Blue + position * (endColor.Blue - startColor.Blue);

            return Color.FromRgb(r, g, b);
        }
        #endregion
    }
}
