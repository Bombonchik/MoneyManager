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
    public class Account : TableData
    {
        private string identifier = "";
   
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public decimal Balance { get; set; }
        public string Identifier 
        { 
            get 
            {
                //if (identifier is null) return null;
               // if (Id == 0) return identifier;     // AddINotifyPropertyChangedInterface calls getter when I set value using binding so this fixes short return
                if (identifier.Length <= 10)
                    return identifier;
                else
                    return $"{identifier.Substring(0, 4)}**{identifier.Substring(identifier.Length - 4)}";
            } 
            set => identifier = value; 
        }
        [Ignore]
        public string FullIdentifier { get => identifier; set => identifier = value; }

        [NotNull]
        public string Type { get; set; }
        [ForeignKey(typeof(AccountView))]
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public int AccoutViewId { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}, Balance: {Balance:C}, Identifier: {Identifier}, Type: {Type}, AccountViewId: {AccoutViewId}";
        }
    }
}
