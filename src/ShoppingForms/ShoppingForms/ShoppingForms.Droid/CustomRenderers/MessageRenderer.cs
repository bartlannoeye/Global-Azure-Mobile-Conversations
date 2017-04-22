using Android.Content;
using Android.Views;
using Android.Widget;
using FormsChatInterface.Services;
using ShoppingForms.Controls;
using ShoppingForms.Droid.CustomRenderers;
using ShoppingForms.Models;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(MessageViewCell), typeof(MessageRenderer))]

namespace ShoppingForms.Droid.CustomRenderers
{
    // bug https://bugzilla.xamarin.com/show_bug.cgi?id=38989 CellRenderer instead of ViewCellRenderer
    public class MessageRenderer : CellRenderer
    {
        protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
        {
            var inflatorservice = (LayoutInflater)Forms.Context.GetSystemService(Context.LayoutInflaterService);
            var dataContext = item.BindingContext as BotMessage;

            if (dataContext != null)
            {
                var template = (LinearLayout)inflatorservice.Inflate(dataContext.IsMine()
                            ? Resource.Layout.message_item_owner
                            : Resource.Layout.message_item_opponent, null, false);
                template.FindViewById<TextView>(Resource.Id.nick).Text =
                    (dataContext.IsMine() ? Constants.MyName : Constants.FriendName) + ":";
                template.FindViewById<TextView>(Resource.Id.message).Text =
                    dataContext.Text;
                return template;
            }

            return base.GetCellCore(item, convertView, parent, context);
        }
    }
}