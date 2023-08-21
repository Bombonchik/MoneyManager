using MoneyManager.Abstractions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class CachedAccountsData : TableData
    {
        public DateTime MonthYear { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal MonthExpenses { get; set; }
        public decimal MonthIncome { get; set; }
        public decimal MonthAverageExpense { get; set; }
        public override string ToString()
        {
            return $"CachedAccountsData: Id: {Id}, MonthYear: {MonthYear}, TotalBalance: {TotalBalance}, MonthExpenses: {MonthExpenses}, " +
                $"MonthIncome: {MonthIncome}, MonthAverageExpense: {MonthAverageExpense}";
        }
    }
}
