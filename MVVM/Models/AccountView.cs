using MoneyManager.Abstractions;
using PropertyChanged;
using SQLite;
using Microsoft.Maui.Graphics;
using Microsoft.Maui;

using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace MoneyManager.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class AccountView : TableData
    {

        [ForeignKey(typeof(Account))]
        public int AccountId { get; set; }
        [NotNull]
        public string Icon { get; set; }
        [NotNull]
        public string BackgroundColor { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
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

        public override string ToString()
        {
            return $"AccountId: {AccountId}, Icon: {Icon}, BackgroundColor: {BackgroundColor}";
        }
    }
}
