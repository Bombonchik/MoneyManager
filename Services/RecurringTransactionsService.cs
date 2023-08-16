using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
