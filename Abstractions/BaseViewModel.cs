using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Abstractions
{
    public class BaseViewModel
    {
        protected void OnNewItemSelected<T>(T previousSelectedItem, T currentSelectedItem) where T : ISelectable
        {
            if (previousSelectedItem is not null)
                previousSelectedItem.IsSelected = false;
            currentSelectedItem.IsSelected = true;
        }
    }
}
