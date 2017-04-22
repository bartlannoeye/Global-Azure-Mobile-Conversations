using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ShoppingForms.Models;

namespace ShoppingForms.UWP
{
    public class MessageDataTemplateSelector : ContentControl
    {
        public DataTemplate MyMessageTemplate { get; set; }

        public DataTemplate OpponentTemplate { get; set; }

        public DataTemplate MyImageTemplate { get; set; }

        public DataTemplate OpponentImageTemplate { get; set; }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            var botMessage = newContent as BotMessage;
            if (botMessage != null)
            {
                ContentTemplate = botMessage.IsMine() ? MyMessageTemplate : OpponentTemplate;
            }
        }
    }
}