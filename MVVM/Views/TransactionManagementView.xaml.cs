using MoneyManager.MVVM.ViewModels;

namespace MoneyManager.MVVM.Views;

public partial class TransactionManagementView : ContentPage
{
	public TransactionManagementView()
	{
		InitializeComponent();
		BindingContext = new TransactionManagementViewModel();
	}
}