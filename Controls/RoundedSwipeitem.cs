using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace MoneyManager
{
    public partial class RoundedSwipeItem : SwipeItem
    {
        public static void Handle()
        {
            CustomHandle();
        }

        static partial void CustomHandle();
    }

}
