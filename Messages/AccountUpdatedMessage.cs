using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class AccountUpdatedMessage
    {
        public Account UpdatedAccount { get; }
        public object Sender { get; }

        public AccountUpdatedMessage(Account account, object sender)
        {
            UpdatedAccount = account;
            Sender = sender;
        }
    }

}
