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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AccountsViewModel
    {
        public AccountDisplay CurrentAccountDisplay { get; set; } = new AccountDisplay();
        private List<Account> Accounts { get; set; }
        private List<AccountView> AccountViews { get; set; }
        public int currentAccountNumber = 1;
        
        private static readonly (Color, Color) AccountBackgroundColorRange =  (Color.FromArgb("#49D1E3"), Color.FromArgb("#00008B"));

        public ObservableCollection<AccountDisplay> AccountDisplays { get; set; } = new ObservableCollection<AccountDisplay>();

        public AccountsViewModel() 
        {
            _ = InitializeAsync();
            //FillDataAsync();
        }
        private async Task InitializeAsync()
        {
            AccountDisplays = await GetAccountDisplaysAsync();
        }
        public ICommand DeleteAccountCommand =>
            new Command(() =>
            {

            });


        private void GenerateNewAccount()
        {
            CurrentAccountDisplay.Account = new Faker<Account>()
                .RuleFor(a => a.Name, f =>
                {
                    var name = f.Company.CompanyName();
                    return name.Length <= 16 ? name : name.Substring(0, 16);
                })
                .RuleFor(a => a.Balance, f => f.Finance.Amount())
                .RuleFor(a => a.Identifier, f => f.Random.AlphaNumeric(10))
                .RuleFor(a => a.Type, f => f.PickRandom(new[] { "Cash", "Card", "Crypto", "Stocks" }))
                .RuleFor(a => a.AccoutViewId, f => currentAccountNumber)
                .Generate();
        }
        private void GenerateNewAccountView()
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
        private async void FillDataAsync()
        {
            for (int i = 0; i < DisplayConstants.IconGlyphs.Count; i++)
            {
                CurrentAccountDisplay = new AccountDisplay();
                GenerateNewAccount();
                GenerateNewAccountView();
                currentAccountNumber++;
                await App.AccountsRepo.SaveItemAsync(CurrentAccountDisplay.Account);
                await App.AccountViewsRepo.SaveItemAsync(CurrentAccountDisplay.AccountView);
                await Console.Out.WriteLineAsync(App.AccountsRepo.StatusMessage);
                await Console.Out.WriteLineAsync(App.AccountViewsRepo.StatusMessage);
                AccountDisplays.Add(CurrentAccountDisplay);
                Debug.WriteLine(CurrentAccountDisplay.ToString());
            }

            
                

                //new Account
                //{
                //    Name = "Cash",
                //    Balance = AccountBalance,
                //    Type = "Card",
                //    Icon = "bars.json",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "50%"
                //}
                
                //new Account
                //{
                //    Name = "Bank Account",
                //    Balance = AccountBalance,
                //    Type = "Card",
                //    Icon = "finance.svg",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "5%"
                //},
                //new Account
                //{
                //    Name = "Main Bank",
                //    Balance = "$10,000,000.00",
                //    Type = "Card",
                //    Icon = "account_balance.svg",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "5%"
                //},
                //new Account
                //{
                //    Name = "Metamask",
                //    Balance = AccountBalance,
                //    Type = "Crypto",
                //    Icon = "receipt_long.svg",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "5%"
                //},
                //new Account
                //{
                //    Name = "Metamask wallet",
                //    Balance = AccountBalance,
                //    Type = "Card",
                //    Icon = "finance.svg",
                //    Identifier = "1245**1252",
                //    Percentage = "5%"
                //},new Account
                //{
                //    Name = "Cash",
                //    Balance = AccountBalance,
                //    Type = "Card",
                //    Icon = "finance.svg",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "50%"
                //},new Account
                //{
                //    Name = AccountName,
                //    Balance = AccountBalance,
                //    Type = "Card",
                //    Icon = "finance.svg",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "5%"
                //},new Account
                //{
                //    Name = AccountName,
                //    Balance = AccountBalance,
                //    Type = "Card",
                //    Icon = "finance.svg",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "5%"
                //},new Account
                //{
                //    Name = AccountName,
                //    Balance = AccountBalance,
                //    Type = "Card",
                //    Icon = "finance.svg",
                //    Identifier = "1245 ** 1252",
                //    Percentage = "5%"
                //},new Account
                //{
                //    Name = AccountName,
                //    Balance = AccountBalance,
                //    Type = "Cash",
                //    Icon = "finance.svg",
                //    Identifier = "1245 ** 1252",
                    //    Percentage = "5%"
                    //},new Account
                    //{
                    //    Name = AccountName,
                    //    Balance = AccountBalance,
                    //    Type = "Card",
                    //    Icon = "finance.svg",
                    //    Identifier = "1245 ** 1252",
                    //    Percentage = "5%"
                    //},

        }
    }
}
