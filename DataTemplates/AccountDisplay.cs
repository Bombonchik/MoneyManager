using MoneyManager.MVVM.Models;

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
