﻿using MoneyManager.Abstractions;
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
    public class AccountManagementViewModel : BaseViewModel
    {
        #region Properties
        private GlyphView selectedIcon;
        public bool IsEditMode { get; private set; }
        // Represents the account being created or edited
        public AccountDisplay NewAccountDisplay { get; set; }
        public string ViewHeader { get; set; }
        // Text for the operation button (either "Save" or "Add")
        public string OperationName { get; set; }
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
                    OnNewItemSelected(selectedIcon, value);
                    selectedIcon = value;

                }
            }
        }
        public Action<AccountDisplay> AccountSavedCallback { get; set; }
        public Action OperationCanceledCallback { get; set; }
        #endregion
        #region Commands
        public ICommand SaveAccountCommand =>
            new Command(async () =>
            {
                if (string.IsNullOrEmpty(NewAccountDisplay.Account.Name) ||
                NewAccountDisplay.Account.Balance < 0 ||
                string.IsNullOrEmpty(SelectedAccountType) ||
                SelectedIcon is null) 
                {
                    await Application.Current.MainPage.DisplayAlert($"Unable to {(IsEditMode ? "save" : "create")} an account", 
                        "Fill in the account name, its nonnegative balance, select the account type and icon", "OK");
                    return;
                }
                NewAccountDisplay.Account.Type = SelectedAccountType;
                Debug.WriteLine(NewAccountDisplay.AccountView.Icon == selectedIcon.Glyph); 
                NewAccountDisplay.AccountView.Icon = selectedIcon.Glyph;
                if (!IsEditMode)
                    await ProcessSavingInNormalMode();
                Debug.WriteLine($"Save: {NewAccountDisplay.Account}");
                Debug.WriteLine($"Save: {NewAccountDisplay.AccountView}");
                ClosePage(NewAccountDisplay);
            });
        public ICommand CancelCommand =>
            new Command( () => {
                ClosePage();
            });
        #endregion
        public AccountManagementViewModel(AccountDisplay existingAccount = null)
        {
            var iconGlyphs = DisplayConstants.IconGlyphs;
            IconGlyphs = new();
            foreach (var iconGlyph in iconGlyphs)
            {
                IconGlyphs.Add(new GlyphView { Glyph = iconGlyph, IsSelected = false });
            }
            if (existingAccount is not null)
                InitializeEditMode(existingAccount);
            else
                InitializeNormalMode();
        }
        #region Private Methods
        private void ClosePage(AccountDisplay newAccountDisplay = null)
        {
            if (newAccountDisplay is not null)
                AccountSavedCallback?.Invoke(newAccountDisplay);
            else
                OperationCanceledCallback?.Invoke();
            Shell.Current.Navigation.PopAsync();
        }
        // Helper method to process saving in normal (non-edit) mode
        private async Task ProcessSavingInNormalMode()
        {
            var AccountWithHighestId = await App.AccountsRepo.GetHighestItemByPropertyAsync("Id");
            var DeletedAccountWithHighestDeletedId = await App.DeletedAccountRepo.GetHighestItemByPropertyAsync("DeletedAccountId");
            int nextAccountId = 0;
            if (AccountWithHighestId is null || DeletedAccountWithHighestDeletedId is null)
            {
                if (AccountWithHighestId is null && DeletedAccountWithHighestDeletedId is null)
                    nextAccountId = 1;
                else if (AccountWithHighestId is null)
                    nextAccountId = DeletedAccountWithHighestDeletedId.DeletedAccountId + 1;
                else
                    nextAccountId = AccountWithHighestId.Id + 1;
            }
            else
                nextAccountId = Math.Max(DeletedAccountWithHighestDeletedId.DeletedAccountId, AccountWithHighestId.Id) + 1;
            NewAccountDisplay.Account.AccountViewId = nextAccountId;
            NewAccountDisplay.AccountView.AccountId = nextAccountId;
            Debug.WriteLine($"nextAccountId: {nextAccountId}");
            NewAccountDisplay.AccountView.BackgroundColor = App.ColorService.GetColorFromGradient(DisplayConstants.AccountBackgroundColorRange).ToHex();
        }
        private void InitializeEditMode(AccountDisplay existingAccount)
        {
            IsEditMode = true;
            NewAccountDisplay = existingAccount;
            var matchingGlyph = IconGlyphs.FirstOrDefault(glyph => glyph.Glyph == NewAccountDisplay.AccountView.Icon);
            if (matchingGlyph is not null)
                SelectedIcon = matchingGlyph;
            SelectedAccountType = NewAccountDisplay.Account.Type;
            ViewHeader = "Edit your account details";
            OperationName = "Save";
            
        }
        private void InitializeNormalMode()
        {
            IsEditMode = false;
            NewAccountDisplay = new AccountDisplay { Account = new Account(), AccountView = new AccountView() };
            ViewHeader = "Fill in your account details";
            OperationName = "Add";
        }
        #endregion
    }

}

