using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.DataTemplates
{
    public class AccountDisplay
    {
        public Account Account { get; set; }
        public AccountView AccountView { get; set; }
        public override string ToString()
        {
            return $"Account: {Account?.ToString() ?? "null"}, AccountView: {AccountView?.ToString() ?? "null"}";
        }
    }
}
