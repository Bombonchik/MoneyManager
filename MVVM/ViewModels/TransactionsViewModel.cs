using CommunityToolkit.Mvvm.Messaging;
using MoneyManager.Abstractions;
using MoneyManager.Constants;
using MoneyManager.DataTemplates;
using MoneyManager.Messages;
using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionsViewModel : BaseViewModel
    {
        public ObservableCollection<DayTransactionGroup> DayTransactionGroups { get; set; }
        private readonly IMessenger Messenger = WeakReferenceMessenger.Default;
        private Dictionary<int, Transaction> TransactionLookup { get; set; }
        private ObservableCollection<TransactionDisplay> TransactionDisplays { get; set; }
        private Dictionary<int, Account> AccountLookup { get; set; }
        private Dictionary<int, AccountNameExtractor> AccountNameExtractorLookup { get; set; } = new Dictionary<int, AccountNameExtractor>();
        private Dictionary<int, CategoryNameExtractor> CategoryNameExtractorLookup { get; set; } = new Dictionary<int, CategoryNameExtractor>();
        private Dictionary<int, DeletedAccount> DeletedAccountLookup { get; set; }
        public Dictionary<int, Category> CategoryLookup { get; set; }
        //TaskCompletionSource objects
        private TaskCompletionSource<bool> accountsReceivedTcs = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> deletedAccountsReceivedTcs = new TaskCompletionSource<bool>();
        public TransactionsViewModel() 
        {
            _ = InitializeAsync();
        }
        private async Task InitializeAsync()
        {

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            GetCategories();
            Messenger.Register<AccountUpdatedMessage>(this, OnAccountUpdated);
            Messenger.Register<AccountDeletedMessage>(this, OnAccountDeleted);
            Messenger.Register<ResponseAccountsMessage>(this, HandleAccountResponse);
            Messenger.Register<ResponseDeletedAccountsMessage>(this, HandleDeletedAccountResponse);
            Messenger.Register<TransactionAddedMessage>(this, OnTransactionAdded);
            Messenger.Send(new RequestAccountsMessage(this));
            Messenger.Send(new RequestDeletedAccountsMessage(this));
            await ProcessTransactions();
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;

            Debug.WriteLine($"Execution time: {elapsed.TotalMilliseconds} ms");
        }

        private void OnTransactionAdded(object recipient, TransactionAddedMessage message)
        {
            if (message.Sender == this) return;
            var transactionDisplay = GetTransactionDisplay(message.NewTransation);
            AddTransactionDisplayToGroup(transactionDisplay);

        }

        private void OnAccountUpdated(object recipient, AccountUpdatedMessage message)
        {
            if (message.Sender == this) return;
            if (AccountNameExtractorLookup.ContainsKey(message.UpdatedAccount.Id))
                AccountNameExtractorLookup[message.UpdatedAccount.Id].UpdateAccountName();
        }

        public ICommand OpenEditTransactionCommand =>
            new Command<TransactionDisplay>(transactionDisplay =>
            {
                var x = transactionDisplay.Transaction;
            });

        private void OnAccountDeleted(object recipient, AccountDeletedMessage message)
        {
            if (message.Sender == this) return;
            if (AccountNameExtractorLookup.ContainsKey(message.DeletedAccount.DeletedAccountId))
                AccountNameExtractorLookup[message.DeletedAccount.DeletedAccountId].UpdateAccountName();
        }

        private void HandleAccountResponse(object recipient, ResponseAccountsMessage message)
        {
            if (message.Sender != this) return;
            AccountLookup = message.Accounts;
            accountsReceivedTcs.SetResult(true);
        }
        private void HandleDeletedAccountResponse(object recipient, ResponseDeletedAccountsMessage message)
        {
            if (message.Sender != this) return;
            DeletedAccountLookup = message.DeletedAccounts;
            deletedAccountsReceivedTcs.SetResult(true);
        }

        private async Task ProcessTransactions()
        {
            await Task.WhenAll(accountsReceivedTcs.Task, deletedAccountsReceivedTcs.Task);
            DayTransactionGroups = await GetDayTransactionGroupsAsync();
            TransactionLookup = new Dictionary<int, Transaction>();
            foreach (var transactionDisplay in TransactionDisplays)
                TransactionLookup[transactionDisplay.Transaction.Id] = transactionDisplay.Transaction;
        }
        private async Task<ObservableCollection<DayTransactionGroup>> GetDayTransactionGroupsAsync()
        {
            TransactionDisplays = await GetTransactionDisplaysAsync();
            var sortedTransactionDisplays = TransactionDisplays.OrderByDescending(x => x.Transaction.DateTime);
            var dayTransactionGroups = new ObservableCollection<DayTransactionGroup>();
            if (TransactionDisplays.Count == 0)
                return dayTransactionGroups;
            DateTime currentDay = sortedTransactionDisplays.First().Transaction.DateTime.Date;
            var currentDayTransactionGroup = new DayTransactionGroup { Date = currentDay, DayTransactions = new ObservableCollection<TransactionDisplay>()};
            foreach (var transactionDisplay in sortedTransactionDisplays)
            {
                if (transactionDisplay.Transaction.DateTime.Date != currentDay)
                {
                    currentDayTransactionGroup.DayTransactions = new ObservableCollection<TransactionDisplay>(currentDayTransactionGroup.DayTransactions.Reverse());
                    dayTransactionGroups.Add(currentDayTransactionGroup);
                    currentDay = transactionDisplay.Transaction.DateTime.Date;
                    currentDayTransactionGroup = new DayTransactionGroup { Date = currentDay, DayTransactions = new ObservableCollection<TransactionDisplay>() };
                }
                currentDayTransactionGroup.DayTransactions.Add(transactionDisplay);
            }
            currentDayTransactionGroup.DayTransactions = new ObservableCollection<TransactionDisplay>(currentDayTransactionGroup.DayTransactions.Reverse());
            dayTransactionGroups.Add(currentDayTransactionGroup);
            return dayTransactionGroups;
        }
        public void AddTransactionDisplayToGroup(TransactionDisplay newTransactionDisplay)
        {
            // Find the DayTransactionGroup for the given date
            var targetGroup = DayTransactionGroups.FirstOrDefault(group => group.Date == newTransactionDisplay.Transaction.DateTime.Date);

            // If the group doesn't exist, create a new one
            if (targetGroup is null)
            {
                targetGroup = new DayTransactionGroup
                {
                    Date = newTransactionDisplay.Transaction.DateTime.Date,
                    DayTransactions = new ObservableCollection<TransactionDisplay>()
                };

                // Find the correct position to insert the new group (assuming DayTransactionGroups is sorted in descending order)
                int groupInsertIndex = DayTransactionGroups.ToList().FindIndex(g => g.Date < targetGroup.Date);
                if (groupInsertIndex == -1)
                    DayTransactionGroups.Add(targetGroup);
                else
                    DayTransactionGroups.Insert(groupInsertIndex, targetGroup);
            }

            // Find the correct position to insert the new TransactionDisplay within the group (assuming DayTransactions is sorted in ascending order)
            int transactionInsertIndex = targetGroup.DayTransactions.ToList().FindIndex(td => td.Transaction.DateTime > newTransactionDisplay.Transaction.DateTime);
            if (transactionInsertIndex == -1)
            {
                targetGroup.DayTransactions.Add(newTransactionDisplay);
            }
            else
            {
                targetGroup.DayTransactions.Insert(transactionInsertIndex, newTransactionDisplay);
            }
        }


        private async Task<ObservableCollection<TransactionDisplay>> GetTransactionDisplaysAsync()
        {
            var transactions = await GetTransactionsAsync();
            var transactionDisplays = new ObservableCollection<TransactionDisplay>();
            foreach (var transaction in transactions)
                transactionDisplays.Add(GetTransactionDisplay(transaction));
            return transactionDisplays;
        }
        private TransactionDisplay GetTransactionDisplay(Transaction transaction)
        {
            TransactionDisplay transactionDisplay = new TransactionDisplay(); 
            try
            {
                var sourceAccountNameExtractor = ExtractAccountName(transaction.SourceAccountId);

                AccountNameExtractor destinationAccountNameExtractor = null;
                if (transaction.Type == TransactionType.Transfer)
                    destinationAccountNameExtractor = ExtractAccountName(transaction.DestinationAccountId.Value);

                CategoryNameExtractor categoryNameExtractor = null;
                if (transaction.Type == TransactionType.Income || transaction.Type == TransactionType.Expense)
                    categoryNameExtractor = ExtractCategoryName(transaction.CategoryId.Value);

                transactionDisplay = new TransactionDisplay
                {
                    Transaction = transaction,
                    TransactionView = new TransactionView
                    {
                        SourceAccountNameExtractor = sourceAccountNameExtractor,
                        DestinationAccountNameExtractor = destinationAccountNameExtractor,
                        CategoryNameExtractor = categoryNameExtractor,
                    },
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return transactionDisplay;
        }
        private AccountNameExtractor ExtractAccountName(int accountId)
        {
            if (AccountNameExtractorLookup.ContainsKey(accountId)) return AccountNameExtractorLookup[accountId];
            var accountNameExtractor = new AccountNameExtractor();

            if (AccountLookup.ContainsKey(accountId))
                accountNameExtractor.Account = AccountLookup[accountId];
            else if (DeletedAccountLookup.ContainsKey(accountId))
                accountNameExtractor.DeletedAccount = DeletedAccountLookup[accountId];
            else
                throw new Exception($"Error: Account with Id: {accountId} was not found");
            AccountNameExtractorLookup.Add(accountId, accountNameExtractor);
            return accountNameExtractor;
        }
        private CategoryNameExtractor ExtractCategoryName(int categoryId)
        {
            if (CategoryNameExtractorLookup.ContainsKey(categoryId)) return CategoryNameExtractorLookup[categoryId];
            var categoryNameExtractor = new CategoryNameExtractor();

            if (CategoryLookup.ContainsKey(categoryId))
                categoryNameExtractor.Category = CategoryLookup[categoryId];
            else
                throw new Exception($"Error: Category with Id: {categoryId} was not found");
            CategoryNameExtractorLookup.Add(categoryId, categoryNameExtractor);
            return categoryNameExtractor;
        }
        private void GetCategories()
        {
            CategoryLookup = CategoryConstants.CategoryLookup;
        }
        private async Task<List<Transaction>> GetTransactionsAsync()
        {
            return await App.TransactionsRepo.GetItemsAsync();
        }
    }
}
