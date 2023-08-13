using Microsoft.Maui.Controls;
using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class AccountsView : ContentPage
{
	public AccountsView()
	{
		InitializeComponent();
		BindingContext = new AccountsViewModel();
	}
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        //await Task.Delay(100);
        //MyCollectionView.InvalidateLayout();


    }

}