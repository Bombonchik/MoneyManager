using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Services
{
    public class AccountService
    {
        private SortedDictionary<int, Account> Accounts;
        private SortedDictionary<int, Account> LastUpdatedAccounts;
        private SortedDictionary<int, DeletedAccount> DeletedAccounts;
        private SortedDictionary<int, DeletedAccount> LastDeletedAccounts;
        public AccountService()
        {
            _ = InitializeAsync();
        }
        public List<Account> GetAccounts()
        {
            return Accounts.Values.ToList();
        }
        public List<Account> GetLastUpdatedAccounts()
        {
            return LastUpdatedAccounts.Values.ToList();
        }
        private async Task InitializeAsync()
        {
            Accounts = new SortedDictionary<int, Account>();
            LastUpdatedAccounts = new SortedDictionary<int, Account>();
            DeletedAccounts = new SortedDictionary<int, DeletedAccount>();
            LastDeletedAccounts = new SortedDictionary<int, DeletedAccount>();
            var accounts = await GetAccountsAsync();
            foreach (var account in accounts)
            {
                Accounts.Add(account.Id, account);
            }
            var deletedAccounts = await GetDeletedAccountsAsync();
            foreach (var deletedAccount in deletedAccounts)
            {
                DeletedAccounts.Add(deletedAccount.Id, deletedAccount);
            }
        }
        public void ClearLastUpdatedAccount()
        {
            LastUpdatedAccounts.Clear();
        }
        private async Task<List<Account>> GetAccountsAsync()
        {
            return await App.AccountsRepo.GetItemsAsync();
        }
        private async Task<List<DeletedAccount>> GetDeletedAccountsAsync()
        {
            return await App.DeletedAccountRepo.GetItemsAsync();
        }
        public async Task SaveAccountAsync(Account account)
        {
            await SaveAsync(account);
        }
        public async Task DeleteAccountAsync(Account account)
        {
            await DeleteAccountAsync(account);
        }
        private async Task SaveAsync(Account account)
        {
            await App.AccountsRepo.SaveItemAsync(account);
            Accounts.Add(account.Id, account);
            LastUpdatedAccounts.Add(account.Id, account);
        }
        private async Task DeleteAsync(Account account)
        {
            await App.RecurringTransactionsService.DeleteRecurringTransactionsAssociatedWithAccountAsync(account);
            await App.AccountsRepo.DeleteItemAsync(account);
            var deletedAccount = new DeletedAccount { DeletedAccountId = account.Id, Name = account.Name };
            await App.DeletedAccountRepo.SaveItemAsync(deletedAccount);
            DeletedAccounts.Add(deletedAccount.Id, deletedAccount);
            Accounts.Remove(account.Id);
            LastDeletedAccounts.Add(deletedAccount.Id, deletedAccount);
        }
    }
}
