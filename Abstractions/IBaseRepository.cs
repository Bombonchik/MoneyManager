using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
          Task DeleteItemAsync(T item);
    }
}
