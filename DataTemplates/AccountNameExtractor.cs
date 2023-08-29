using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.DataTemplates
{
    [AddINotifyPropertyChangedInterface]
    public class AccountNameExtractor
    {
        private string accountName;

        private Account account;
        public Account Account
        {
            get => account;
            set
            {
                account = value;
                UpdateAccountName();
            }
        }

        private DeletedAccount deletedAccount;
        public DeletedAccount DeletedAccount
        {
            get => deletedAccount;
            set
            {
                deletedAccount = value;
                UpdateAccountName();
            }
        }

        public string AccountName
        {
            get => accountName;
            private set => accountName = value;
        }

        public void UpdateAccountName()
        {
            if (Account is null && DeletedAccount is null)
                AccountName = $"Error: Account with Id: {Account?.Id} was not found";
            else
                AccountName = Account is not null ? Account.Name : DeletedAccount.Name;
            Debug.WriteLine(AccountName);
        }
    }
}
