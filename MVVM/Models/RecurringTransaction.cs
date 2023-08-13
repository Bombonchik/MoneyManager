using MoneyManager.Abstractions;
using MoneyManager.MVVM.Models;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.MVVM.Models
{
    public enum FrequencyType
    {
        Daily,
        Weekly,
        BiWeekly,  // Every two weeks
        Monthly,
        Quarterly, // Every three months
        SemiAnnually, // Every six months
        Annually
    }

    public class RecurringTransaction : TableData
    {
        [ForeignKey(typeof(Transaction))]
        public int MainTransactionId { get; set; }
        [NotNull]
        public DateTime StartDate { get; set; }
        [NotNull]
        public FrequencyType Frequency { get; set; }
        [NotNull]
        public DateTime NextDueDate { get; set; }
    }
}

//public DateTime CalculateNextDueDate(DateTime startDate, FrequencyType frequency)
//{
//    DateTime nextDueDate;

//    switch (frequency)
//    {
//        case FrequencyType.Daily:
//            nextDueDate = startDate.AddDays(1);
//            break;

//        case FrequencyType.Weekly:
//            nextDueDate = startDate.AddDays(7);
//            break;

//        case FrequencyType.BiWeekly:
//            nextDueDate = startDate.AddDays(14);
//            break;

//        case FrequencyType.Monthly:
//            nextDueDate = startDate.AddMonths(1);
//            break;

//        case FrequencyType.Quarterly:
//            nextDueDate = startDate.AddMonths(3);
//            break;

//        case FrequencyType.SemiAnnually:
//            nextDueDate = startDate.AddMonths(6);
//            break;

//        case FrequencyType.Annually:
//            nextDueDate = startDate.AddYears(1);
//            break;

//        default:
//            throw new InvalidOperationException($"Unsupported frequency type: {frequency}");
//    }

//    return nextDueDate;
//}
