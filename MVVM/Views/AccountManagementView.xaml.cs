using MoneyManager.DataTemplates;
using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class AccountManagementView : ContentPage
{
	public AccountManagementView(AccountDisplay existingAccount = null)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        BindingContext = (existingAccount is null) ? new AccountManagementViewModel() : new AccountManagementViewModel(existingAccount);
    }
}