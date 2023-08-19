using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.DataTemplates
{
    [AddINotifyPropertyChangedInterface]
    public class GlyphView
    {
        public string Glyph { get; set; }
        public bool IsSelected { get; set; }
        public Color StrokeColor
        {
            get
            {
                if (IsSelected)
                {
                    return Application.Current.PlatformAppTheme == AppTheme.Light ?
                        Color.FromArgb("#852BD4") :
                        Colors.White;
                }
                return Colors.Transparent;
            }
        }
    }
}
