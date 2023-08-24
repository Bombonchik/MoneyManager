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
        public Account DeletedAccount { get; }
        public object Sender { get; }

        public AccountDeletedMessage(Account deletedAccount, object sender)
        {
            DeletedAccount = deletedAccount;
            Sender = sender;
        }
    }
}
