using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class TransactionManagementView : ContentPage
{
    public TransactionManagementViewModel ViewModel => BindingContext as TransactionManagementViewModel;
    public TransactionManagementView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = new TransactionManagementViewModel();
	}

}