﻿using MoneyManager.Abstractions;
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
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public decimal Balance { get; set; }
        public string Identifier { get; set; }
        [NotNull]
        public string Type { get; set; }
        [ForeignKey(typeof(AccountView))]
        public int AccountViewId { get; set; }
        public override string ToString()
        {
            return $"Account: Id: {Id}, Name: {Name}, Balance: {Balance:C}, Identifier: {Identifier}, Type: {Type}, AccountViewId: {AccountViewId}";
        }
        public Account Clone()
        {
            return new Account
            {
                Id = this.Id,
                Name = this.Name,
                Balance = this.Balance,
                Identifier = this.Identifier,
                Type = this.Type,
                AccountViewId = this.AccountViewId,
            };
        }
    }   
}
