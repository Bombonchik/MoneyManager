using SQLite;

namespace MoneyManager
{
    public static class DatabaseConstants

    {
        private const string DBFileName = "MoneyManager.db3";

        public const SQLiteOpenFlags Flags =
             SQLiteOpenFlags.ReadWrite |
             SQLiteOpenFlags.Create |
             SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                return Path
                     .Combine(FileSystem.AppDataDirectory, DBFileName);
            }
        }
    }
}
