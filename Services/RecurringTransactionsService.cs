using MoneyManager.MVVM.Models;

namespace MoneyManager.Services
{
    public class RecurringTransactionsService
    {
        public async Task<CachedAccountsData> ExecuteRecurringTransactionsForMonth(DateTime month)
        {
            if (month > DateTime.Now)
            {
                return await App.CachedAccountsDataRepo.GetLastItemAsync();
            }
            return null;
        }
    }
}
