using MoneyManager.Constants;
using MoneyManager.DataTemplates;
using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AddAccountViewModel
    {
        public AccountDisplay NewAccountDisplay { get; set; }
        public List<string> AccountTypes { get; set; } = new List<string>
        {
            "Card",
            "Credit Card",
            "Debit Card",
            "Virtual Card",
            "Cash",
            "Bank Account",
            "Crypto", 
            "Stocks", 
            "Saving Account",
            "Forex",
            "Precious Metals",
            "Real Estate",
            "Bonds"
        };
        public string SelectedAccountType { get; set; }
        public List<string> IconGlyphs { get; set; } = DisplayConstants.IconGlyphs;
        public string SelectedIcon { get; set;}
    }
    
}
