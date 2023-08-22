using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.DataTemplates
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionDisplay
    {
        public Transaction Transaction { get; set; }
        public TransactionView TransactionView { get; set; }
    }
}
