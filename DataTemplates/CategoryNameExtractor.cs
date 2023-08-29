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
    public class CategoryNameExtractor
    {
        public Category Category { get; set; }
        public string CategoryName 
        { 
            get
            {
                if (Category is null) return $"Error: Category with Id: {Category.Id} was not found";
                return Category.Name;
            }
        }
    }
}
