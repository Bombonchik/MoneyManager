using MoneyManager.MVVM.Models;
using MoneyManager.MVVM.Views;
using MoneyManager.Repositories;
using MoneyManager.Services;

namespace MoneyManager;

public partial class App : Application
{
    public static BaseRepository<Account> AccountsRepo { get; private set; }
    public static BaseRepository<AccountView> AccountViewsRepo { get; private set; }
    public static BaseRepository<Category> CategoriesRepo { get; private set; }
    public static BaseRepository<Transaction> TransactionsRepo { get; private set; }
    public static BaseRepository<RecurringTransaction> RecurringTransactionsRepo { get; private set; }
    public static BaseRepository<CachedAccountsData> CachedAccountsDataRepo { get; private set; }
    public static CachedAccountsDataService CachedAccountsDataService { get; private set; }
    public static RecurringTransactionsService RecurringTransactionsService { get; private set; }

    public App(BaseRepository<Account> accountsRepo,
               BaseRepository<AccountView> accountViewsRepo,
               BaseRepository<Category> categoriesRepo,
               BaseRepository<Transaction> transactionsRepo,
               BaseRepository<RecurringTransaction> recurringTransactionsRepo,
               BaseRepository<CachedAccountsData> cachedAccountsDataRepo,
               CachedAccountsDataService cachedAccountsDataService,
               RecurringTransactionsService recurringTransactionsService
               )
	{
		InitializeComponent();

		MainPage = new AddAccountView();
        AccountsRepo = accountsRepo;
        AccountViewsRepo = accountViewsRepo;
        CategoriesRepo = categoriesRepo;
        TransactionsRepo = transactionsRepo;
        RecurringTransactionsRepo = recurringTransactionsRepo;
        CachedAccountsDataRepo = cachedAccountsDataRepo;
        CachedAccountsDataService = cachedAccountsDataService;
        RecurringTransactionsService = recurringTransactionsService;
    }
}
