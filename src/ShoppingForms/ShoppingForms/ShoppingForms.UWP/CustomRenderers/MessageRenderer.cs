using ShoppingForms.Controls;
using ShoppingForms.UWP.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Application = Windows.UI.Xaml.Application;
using DataTemplate = Windows.UI.Xaml.DataTemplate;

[assembly: ExportRenderer(typeof(MessageViewCell), typeof(MessageRenderer))]

namespace ShoppingForms.UWP.CustomRenderers
{
    public class MessageRenderer : ViewCellRenderer
    {
        public override DataTemplate GetTemplate(Cell cell)
        {
            return Application.Current.Resources["MessageDataTemplate"] as DataTemplate;
        }
    }
}
 