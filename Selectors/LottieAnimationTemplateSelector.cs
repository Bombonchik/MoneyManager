using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Selectors
{
    public class LottieAnimationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LottieAnimationTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return LottieAnimationTemplate;
        }
    }
}
