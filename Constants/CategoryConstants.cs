using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Constants
{
    public static class CategoryConstants
    {
        public static Dictionary<int, Category> CategoryLookup { get; }
        static CategoryConstants()
        {
            CategoryLookup = new Dictionary<int, Category>();
            foreach (var category in IncomeCategories)
                CategoryLookup.Add(category.Id, category);
            foreach (var category in ExpenseCategories)
                CategoryLookup.Add(category.Id, category);
        }
        public static ObservableCollection<Category> IncomeCategories { get; } = new ObservableCollection<Category>
        {
            // Income Categories
            new Category
            {
                Id = 1,
                Name = "Salary",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 2,
                Name = "Business",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 3,
                Name = "Investments",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 4,
                Name = "Rental",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 5,
                Name = "Dividends",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 6,
                Name = "Pension",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 7,
                Name = "Interest",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 8,
                Name = "Freelance",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 9,
                Name = "Sale of Assets",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 10,
                Name = "Social Security",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 11,
                Name = "Grant",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 12,
                Name = "Lottery",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 13,
                Name = "Tax Refund",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 14,
                Name = "Royalties",
                CategoryType = TransactionType.Income
            },
            new Category
            {
                Id = 15,
                Name = "Other",
                CategoryType = TransactionType.Income
            }
        };
        public static ObservableCollection<Category> ExpenseCategories { get; } = new ObservableCollection<Category>
        {
            new Category
            {
                Id = 16,
                Name = "Food",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 17,
                Name = "Housing",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 18,
                Name = "Transport",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 19,
                Name = "Eating Out",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 20,
                Name = "Health",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 21,
                Name = "Fun",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 22,
                Name = "Sports",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 23,
                Name = "Shopping",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 24,
                Name = "Education",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 25,
                Name = "Savings",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 26,
                Name = "Investments",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 27,
                Name = "Family",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 28,
                Name = "Travel",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 29,
                Name = "Pets",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 30,
                Name = "Subscriptions",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 31,
                Name = "Insurance",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 32,
                Name = "Self-Care",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 33,
                Name = "Debt Payments",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 34,
                Name = "Connectivity",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 35,
                Name = "Gifts",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 36,
                Name = "Taxes",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 37,
                Name = "Charity",
                CategoryType = TransactionType.Expense
            },
            new Category
            {
                Id = 38,
                Name = "Other",
                CategoryType = TransactionType.Expense
            }
        };
    }
}
