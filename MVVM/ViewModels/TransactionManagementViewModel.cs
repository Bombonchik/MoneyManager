using MoneyManager.Abstractions;
using MoneyManager.DataTemplates;
using MoneyManager.MVVM.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Transaction = MoneyManager.MVVM.Models.Transaction;

namespace MoneyManager.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionManagementViewModel : BaseViewModel
    {
        private TransactionTypeView selectedTransactionType;
        public bool IsEditMode { get; private set; }
        public TransactionDisplay NewTransactionDisplay { get; set; }
        public TimeSpan SelectedTime { get; set; }

        public TransactionTypeView SelectedTransactionType
        {
            get => selectedTransactionType; set
            {
                if (selectedTransactionType != value)
                {
                    OnNewItemSelected(selectedTransactionType, value);
                    selectedTransactionType = value;
                    TransferIndicator = selectedTransactionType.Type == TransactionType.Transfer;
                }
            }
        }
        public ObservableCollection<TransactionTypeView> TransactionTypes { get; set; }
        public bool TransferIndicator { get; set; }
        public ICommand ClosePageCommand =>
            new Command(() =>
            {
                var y = NewTransactionDisplay;

                (App.Current.MainPage as AppShell).ViewModel.CloseAddPage();
            });
        public TransactionManagementViewModel(TransactionDisplay existingTransactionDisplay = null)
        {
            TransactionTypes = new ();
            foreach (TransactionType transactionType in Enum.GetValues(typeof(TransactionType)))
            {
                TransactionTypes.Add(new TransactionTypeView { Type = transactionType, IsSelected = false  });
            }
            if (existingTransactionDisplay is not null)
                InitializeEditMode(existingTransactionDisplay);
            else
                InitializeNormalMode();
        }
        private void InitializeEditMode(TransactionDisplay existingTransactionDisplay)
        {
            IsEditMode = true;
            NewTransactionDisplay = existingTransactionDisplay;
            SelectedTransactionType = new TransactionTypeView { Type = NewTransactionDisplay.Transaction.Type, IsSelected = true };
            //SelectedTransactionType = TransactionTypes.Where(x => x.Type == existingTransactionDisplay.Transaction.Type).FirstOrDefault();
        }
        private void InitializeNormalMode()
        {
            IsEditMode = false;
            NewTransactionDisplay = new TransactionDisplay { Transaction = new Transaction(), TransactionView = new TransactionView() };
            SelectedTransactionType = TransactionTypes.Where(x => x.Type == TransactionType.Expense).FirstOrDefault();
        }
    }
}
