using MoneyManager.Abstractions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public AccountNameExtractor SourceAccountNameExtractor { get; set; }
        public AccountNameExtractor DestinationAccountNameExtractor { get; set; }
        public CategoryNameExtractor CategoryNameExtractor { get; set; }
        public TransactionView Clone()
        {
            return new TransactionView
            {
                IsSelected = this.IsSelected,
                SourceAccountNameExtractor = this.SourceAccountNameExtractor,
                DestinationAccountNameExtractor = this.DestinationAccountNameExtractor,
            };
        }
    }
}
