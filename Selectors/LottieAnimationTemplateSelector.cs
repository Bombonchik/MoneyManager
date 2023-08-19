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
