using MoneyManager.Constants;
using MoneyManager.DataTemplates;
using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AddAccountViewModel
    {
        private GlyphView selectedIcon;

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
        public List<GlyphView> IconGlyphs { get; set; }

        public GlyphView SelectedIcon
        {
            get => selectedIcon; set
            {
                if (value == null) return;
                if (selectedIcon != value)
                {
                    OnNewAccountSelected(selectedIcon, value);
                    selectedIcon = value;

                }
            }
        }
        public Action OnPageClosedCallback { get; set; }
        public ICommand AddNewAccountCommand =>
            new Command(async () =>
            {
                if (string.IsNullOrEmpty(NewAccountDisplay.Account.Name) ||
                NewAccountDisplay.Account.Balance < 0 ||
                string.IsNullOrEmpty(SelectedAccountType) ||
                SelectedIcon is null) 
                {
                    await Application.Current.MainPage.DisplayAlert("Unable to create an account", 
                        "Fill in the account name, its nonnegative balance, select the account type and icon", "OK");
                    return;
                }
                NewAccountDisplay.Account.Type = SelectedAccountType;
                NewAccountDisplay.AccountView.Icon = selectedIcon.Glyph;
                NewAccountDisplay.Account.AccoutViewId = await App.AccountViewsRepo.GetCountAsync() + 1;
                NewAccountDisplay.AccountView.AccountId = await App.AccountsRepo.GetCountAsync() + 1;
                NewAccountDisplay.AccountView.BackgroundColor = App.ColorService.GetColorFromGradient(DisplayConstants.AccountBackgroundColorRange).ToHex();
                await App.AccountsRepo.SaveItemAsync(NewAccountDisplay.Account);
                await App.AccountViewsRepo.SaveItemAsync(NewAccountDisplay.AccountView);
                ClosePage();
            });
        private void ClosePage()
        {
            OnPageClosedCallback?.Invoke();
            Shell.Current.Navigation.PopAsync();
        }

        public AddAccountViewModel()
        {
            var iconGlyphs = DisplayConstants.IconGlyphs;
            IconGlyphs = new();
            foreach (var iconGlyph in iconGlyphs)
            {
                IconGlyphs.Add(new GlyphView { Glyph = iconGlyph, IsSelected = false });
            }
            selectedIcon = IconGlyphs.FirstOrDefault();
            NewAccountDisplay = new AccountDisplay { Account = new Account(), AccountView = new AccountView()};
        }
        private void OnNewAccountSelected(GlyphView previousSelectedIcon, GlyphView currentSelectedIcon)
        {
            if (previousSelectedIcon is not null)
                previousSelectedIcon.IsSelected = false;
            currentSelectedIcon.IsSelected = true;
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class GlyphView
    {
        public string Glyph {get; set;}
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

