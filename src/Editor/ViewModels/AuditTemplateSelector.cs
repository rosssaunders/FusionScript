using System.Windows;
using System.Windows.Controls;

namespace RxdSolutions.FusionScript.ViewModels
{
    public class AuditTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Collapsed { get; set; }

        public DataTemplate Expanded { get; set; }

        public DataTemplate ComboBoxItem { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var presenter = (ContentPresenter)container;

            if(item is ComboBoxItem)
            {
                return ComboBoxItem;
            }

            if (presenter.TemplatedParent is ComboBox)
            {
                return Collapsed; 
            }
            else
            {
                return Expanded;
            }
        }
    }
}
