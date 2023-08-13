using MoneyManager.Abstractions;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.Models
{
    public enum TransactionType
    {
        Income,
        Expense,
        Transfer
    }

    public class Transaction : TableData
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        // For Income and Expense, this will be the account involved.
        // For Transfer, this will be the source account.
        [ForeignKey(typeof(Account))]
        public int? SourceAccountId { get; set; }

        // For Transfer, this will be the destination account.
        [ForeignKey(typeof(Account))]
        public int? DestinationAccountId { get; set; }

        [ForeignKey(typeof(RecurringTransaction))]
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public int? RecurringTransactionId { get; set; }
        [ForeignKey(typeof(Category))]
        public int? CategoryId { get; set; }

    }
}
