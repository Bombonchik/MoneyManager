using MoneyManager.MVVM.Views;

namespace MoneyManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("addAccountPage", typeof(AddAccountView));
    }
}
