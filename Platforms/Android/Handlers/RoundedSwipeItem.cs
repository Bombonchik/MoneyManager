using Android.Graphics.Drawables;
using Microsoft.Maui;
using MoneyManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    public partial class RoundedSwipeItem : SwipeItem
    {
        static partial void CustomHandle()
        {

           // Microsoft.Maui.Controls.Handlers.Items.SelectableItemsViewHandler<CollectionView>.SelectableItemsViewMapper.AppendToMapping("MyCustomMapping", (handler, view) =>
          //  {

                //var x = (Android.Widget.ListView)handler.PlatformView.DispatchSetSelected;
                //var gradientDrawable = new GradientDrawable();
                //gradientDrawable.SetColor(Android.Graphics.Color.Yellow);
                //gradientDrawable.SetCornerRadius(50); // Adjust this value as needed

                //handler.PlatformView.SetBackgroundDrawable(gradientDrawable);
           // });
            //Microsoft.Maui.Handlers.SwipeItemMenuItemHandler.Mapper.AppendToMapping("MyCustomMapping", (handler, view) =>
            //{
            //    var gradientDrawable = new GradientDrawable();
            //    gradientDrawable.SetColor(Android.Graphics.Color.Yellow);
            //    gradientDrawable.SetCornerRadius(50); // Adjust this value as needed

            //    handler.PlatformView.SetBackgroundDrawable(gradientDrawable);
            //});


            //Microsoft.Maui.Handlers.SwipeItemMenuItemHandler.Mapper.AppendToMapping("MyCustomMapping", (handler, view) =>
            //{
            //    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Yellow);


            //});
        }
    }
}
