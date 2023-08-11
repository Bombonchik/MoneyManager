using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class AccountsView : ContentPage
{
	public AccountsView()
	{
		InitializeComponent();
		BindingContext = new AccountsViewModel();
	}
}