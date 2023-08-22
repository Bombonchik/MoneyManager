using Microsoft.Maui.Controls;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AppShellViewModel
    {
        public int PreviousTabIndex { get; set; }
        public void CloseAddPage()
        {
            var shell = (App.Current.MainPage as AppShell);
            if (shell.Items[0] is TabBar tabBar)
            {
                tabBar.CurrentItem = tabBar.Items[PreviousTabIndex];
            }
        }
    }
}
