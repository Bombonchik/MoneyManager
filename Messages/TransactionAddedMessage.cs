using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class TransactionAddedMessage
    {
        public Transaction NewTransation { get; }
        public object Sender { get; }

        public TransactionAddedMessage(Transaction transaction, object sender)
        {
            NewTransation = transaction;
            Sender = sender;
        }
    }
}
