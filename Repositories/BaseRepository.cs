

using MoneyManager.Abstractions;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using System.Linq.Expressions;

namespace MoneyManager.Repositories
{
    public class BaseRepository<T> :
        IBaseRepository<T> where T : TableData, new()
    {
        SQLiteAsyncConnection connection;
        public string StatusMessage { get; set; }

        public BaseRepository()
        {
            connection =
                new SQLiteAsyncConnection(DatabaseConstants.DatabasePath,
                DatabaseConstants.Flags);
            connection.CreateTableAsync<T>().Wait();
        }

        public async Task DeleteItemAsync(T item)
        {
            try
            {
                await connection.DeleteAsync(item, true);
            }
            catch (Exception ex)
            {
                StatusMessage =
                        $"Error: {ex.Message}";
            }
        }

        public void Dispose()
        {
            connection.CloseAsync();
        }

        public async Task<T> GetItemAsync(int id)
        {
            try
            {
                return await connection.Table<T>().FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            return null;
        }
        public async Task<T> GetLastItemAsync()
        {
            try
            {
                return await connection.Table<T>().OrderByDescending(item => item.Id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            return null;
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await connection.Table<T>().Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            return null;
        }
        public async Task<List<T>> GetItemsAsync()
        {
            try
            {
                return await connection.Table<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            return null;
        }

        public async Task<List<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await connection.Table<T>().Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            return null;
        }

        public async Task SaveItemAsync(T item)
        {
            int result = 0;
            try
            {
                if (item.Id != 0)
                {
                    result = await connection.UpdateAsync(item);
                    StatusMessage = $"{result} row(s) updated";
                }
                else
                {
                    result = await connection.InsertAsync(item);
                    StatusMessage = $"{result} row(s) added";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        public async Task SaveItemWithChildrenAsync(T item, bool recursive = false)
        {
            await connection.InsertWithChildrenAsync(item, recursive);
        }

        public async Task<List<T>> GetItemsWithChildrenAsync()
        {
            try
            {
                return await connection.GetAllWithChildrenAsync<T>();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            return null;
        }

        public async Task<int> GetCountAsync()
        {
            return await connection.Table<T>().CountAsync();
        }
    }
}
