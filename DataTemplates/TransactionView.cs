using MoneyManager.Abstractions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.DataTemplates
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionView : ISelectable
    {
        public bool IsSelected { get; set; }
        public Color StrokeColor
        {
            get
            {
                if (IsSelected)
                {
                    return Application.Current.PlatformAppTheme == AppTheme.Light ?
                        Color.FromArgb("#10BFFF") :
                        Color.FromArgb("#852BD4");

                }
                return Colors.Transparent;
            }
        }
    }
}
