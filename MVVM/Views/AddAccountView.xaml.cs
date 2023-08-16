using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class AddAccountView : ContentPage
{
	public AddAccountView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = new AddAccountViewModel();
    }
}