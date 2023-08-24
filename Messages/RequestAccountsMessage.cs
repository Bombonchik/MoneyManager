using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Messages
{
    public class RequestAccountsMessage
    {
        public object Sender { get; }

        public RequestAccountsMessage(object sender)
        {
            Sender = sender;
        }
    }
}
