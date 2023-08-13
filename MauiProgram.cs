using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MoneyManager.MVVM.Models;
using MoneyManager.Repositories;

namespace MoneyManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Roboto-Black.ttf", "Strong");
                fonts.AddFont("LibreFranklin-Regular.ttf", "Regular");
                fonts.AddFont("PaymentFont.ttf", "PaymentFont");
            });
        builder.Services.AddSingleton<BaseRepository<Account>>();
        builder.Services.AddSingleton<BaseRepository<AccountView>>();
        builder.Services.AddSingleton<BaseRepository<Category>>();
        builder.Services.AddSingleton<BaseRepository<Transaction>>();
        builder.Services.AddSingleton<BaseRepository<RecurringTransaction>>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
