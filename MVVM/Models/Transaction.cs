using MoneyManager.Abstractions;
using PropertyChanged;
using SQLiteNetExtensions.Attributes;

namespace MoneyManager.MVVM.Models
{
    public enum TransactionType
    {
        Income,
        Expense,
        Transfer
    }
    [AddINotifyPropertyChangedInterface]
    public class Transaction : TableData
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string Note { get; set; }

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
