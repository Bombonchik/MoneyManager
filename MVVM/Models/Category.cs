using MoneyManager.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
