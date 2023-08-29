using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class RequestDeletedAccountsMessage
    {
        public object Sender { get; }
        public RequestDeletedAccountsMessage(object sender)
        {
            Sender = sender;
        }
    }
}
