using MoneyManager.Abstractions;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.Models
{
    public class AccountView : TableData
    {
        [ForeignKey(typeof(Account))]
        public int AccountId { get; set; }
        [NotNull]
        public string Icon { get; set; }
        [NotNull]
        public string BackgroundColor { get; set; }
        public override string ToString()
        {
            return $"AccountId: {AccountId}, Icon: {Icon}, BackgroundColor: {BackgroundColor}";
        }
    }
}
