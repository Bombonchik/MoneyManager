using MoneyManager.MVVM.Models;
using MoneyManager.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Services
{
    public class CachedAccountsDataService
    {
        public async Task<CachedAccountsData> GetSafelyLastCashedAccountsDataAsync(Func<decimal> getCurrentTotalBalance)
        {
            var allCachedAccountsData = await GetAllCashedAccountsDataAsync();
            CachedAccountsData cachedAccountsData = new CachedAccountsData();

            if (allCachedAccountsData is null || allCachedAccountsData.Count == 0)
            {
                cachedAccountsData = new CachedAccountsData
                {
                    MonthYear = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    TotalBalance = getCurrentTotalBalance(),
                    MonthAverageExpense = 0,
                    MonthExpenses = 0,
                    MonthIncome = 0,
                };
                await App.CachedAccountsDataRepo.SaveItemAsync(cachedAccountsData);
                return cachedAccountsData;
            }
            var cachedDataCount = allCachedAccountsData.Count();
            var lastDate = allCachedAccountsData[cachedDataCount - 1].MonthYear;
            var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            while (lastDate < currentDate)
            {
                lastDate = lastDate.AddMonths(1); // Move to the next month
                try
                {
                    cachedAccountsData = await App.RecurringTransactionsService.ExecuteRecurringTransactionsForMonthAsync(lastDate);
                    if (cachedAccountsData is not null)
                        await App.CachedAccountsDataRepo.SaveItemAsync(cachedAccountsData);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            }
            return await GetLastCashedAccountsDataAsync();
        }


        public static async Task<List<CachedAccountsData>> GetAllCashedAccountsDataAsync()
        {
            return await App.CachedAccountsDataRepo.GetItemsAsync();
        }
        public static async Task<CachedAccountsData> GetLastCashedAccountsDataAsync()
        {
            return await App.CachedAccountsDataRepo.GetLastItemAsync();
        }
    }
}
