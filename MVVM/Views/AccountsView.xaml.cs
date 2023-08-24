using Bogus;
using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class AccountsView : ContentPage
{
    public AccountsViewModel ViewModel => BindingContext as AccountsViewModel;
    public AccountsView()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = new AccountsViewModel();
    }


}