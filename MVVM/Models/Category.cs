using MoneyManager.Abstractions;
using SQLite;

namespace MoneyManager.MVVM.Models
{
    public class Category : TableData
    {
        [NotNull, Unique]
        public string Name { get; set; }
        [NotNull]
        public TransactionType CategoryType { get; set; }
    }
}
