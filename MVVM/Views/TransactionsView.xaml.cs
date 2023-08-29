using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class TransactionsView : ContentPage
{
    public TransactionsView()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = new TransactionsViewModel();
    }
}