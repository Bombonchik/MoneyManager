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
        //AccountsBorder.IsVisible = true;
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        AccountsBorder.IsVisible = !AccountsBorder.IsVisible;
    }
}