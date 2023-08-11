using MoneyManager.MVVM.Views;

namespace MoneyManager;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
