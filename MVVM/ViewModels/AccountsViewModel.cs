using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.ViewModels
{
    public class AccountsViewModel
    {
        public string AccountName { get; set; } = "Cash";
        public string AccountBalance { get; set; } = "$1020";
        public ObservableCollection<Account> Accounts { get; set; }
        public AccountsViewModel() 
        {
            Accounts = new ObservableCollection<Account>
            {
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
                new Account
                {
                    Name = AccountName,
                    Balance = AccountBalance
                },
            };
        }
    }
}
