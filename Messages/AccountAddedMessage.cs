using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class AccountAddedMessage
    {
        public Account NewAccount { get; }
        public object Sender { get; }

        public AccountAddedMessage(Account account, object sender)
        {
            NewAccount = account;
            Sender = sender;
        }
    }
}
