using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.DataTemplates
{
    [AddINotifyPropertyChangedInterface]
    public class DayTransactionGroup
    {
        public DateTime Date { get; set; }
        public ObservableCollection<TransactionDisplay> DayTransactions { get; set; }
    }
}
