using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class ResponseAccountsMessage
    {
        public Dictionary<int, Account> Accounts { get; }
        public object Sender { get; }

        public ResponseAccountsMessage(Dictionary<int, Account> accounts, object sender)
        {
            Accounts = accounts;
            Sender = sender;
        }
    }
}
