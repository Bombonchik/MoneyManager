using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class ResponseDeletedAccountsMessage
    {
        public Dictionary<int, DeletedAccount> DeletedAccounts { get; }
        public object Sender { get; }
        public ResponseDeletedAccountsMessage(Dictionary<int, DeletedAccount> deletedAccounts, object sender)
        {
            DeletedAccounts = deletedAccounts;
            Sender = sender;
        }
    }
}
