using MoneyManager.MVVM.ViewModels;
using MoneyManager.MVVM.Views;

namespace MoneyManager;

public partial class AppShell : Shell
{
    public AppShellViewModel ViewModel => BindingContext as AppShellViewModel;
    public AppShell()
	{
		InitializeComponent();
        BindingContext = new AppShellViewModel();
        this.Navigating += OnShellNavigating;
        this.Navigated += OnShellNavigated;
    }
    private void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
    {
        if (this.Items[0] is TabBar tabBar)
        {
            var tabIndex = tabBar.Items.IndexOf(tabBar.CurrentItem);
            ViewModel.PreviousTabIndex = tabIndex;
        }
    }

    private async void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
    {
        if (e.Current.Location.OriginalString.Contains("TransactionManagementView"))
        {
            await Navigation.PushAsync(new TransactionManagementView());
        }
    }
}
