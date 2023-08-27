using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class TransactionUpdatedMessage
    {
        public Transaction OldTransation { get; }
        public Transaction UpdatedTransation { get; }
        public object Sender { get; }

        public TransactionUpdatedMessage(Transaction oldTransaction, Transaction updatedTransaction, object sender)
        {
            OldTransation = oldTransaction;
            UpdatedTransation = updatedTransaction;
            Sender = sender;
        }
    }
}
