using System.Linq.Expressions;

namespace MoneyManager.Abstractions
{
    public interface IBaseRepository<T> : IDisposable
          where T : TableData, new()
    {
        Task SaveItemAsync(T item);
        Task SaveItemWithChildrenAsync(T item, bool recursive = false);
        Task<T> GetItemAsync(int id);
        Task<T> GetItemAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetItemsAsync();
        Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetItemsWithChildrenAsync();
        Task<int> GetCountAsync();
        Task DeleteItemAsync(T item);
        Task<T> GetHighestItemByPropertyAsync(string propertyName);
    }
}
