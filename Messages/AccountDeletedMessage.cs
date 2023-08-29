using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class AccountDeletedMessage
    {
        public Account Account { get; }
        public DeletedAccount DeletedAccount { get; }
        public object Sender { get; }

        public AccountDeletedMessage(Account account, DeletedAccount deletedAccount, object sender)
        {
            Account = account;
            DeletedAccount = deletedAccount;
            Sender = sender;
        }
    }
}
