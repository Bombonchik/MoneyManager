using MoneyManager.Abstractions;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class DeletedAccount : TableData
    {
        [NotNull]
        public int DeletedAccountId { get; set; }
        [NotNull]
        public string Name { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, DeletedAccountId: {DeletedAccountId}, Name: {Name}";
        }
    }
}
