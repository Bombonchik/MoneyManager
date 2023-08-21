using MoneyManager.MVVM.Models;

namespace MoneyManager.Services
{
    public class RecurringTransactionsService
    {
        public async Task<CachedAccountsData> ExecuteRecurringTransactionsForMonthAsync(DateTime month)
        {
            if (month > DateTime.Now)
            {
                return await App.CachedAccountsDataRepo.GetLastItemAsync();
            }
            return null;
        }
        public async Task DeleteRecurringTransactionsAssociatedWithAccountAsync(Account account)
        {
            await Task.CompletedTask;
        }
    }
}
