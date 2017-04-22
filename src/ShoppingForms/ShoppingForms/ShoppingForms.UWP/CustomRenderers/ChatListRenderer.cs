using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using ShoppingForms.Controls;
using ShoppingForms.UWP.CustomRenderers;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ChatListView), typeof(ChatListRenderer))]

namespace ShoppingForms.UWP.CustomRenderers
{
    public class ChatListRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            var listView = Control as ListView;

            if (listView != null && listView.ItemsSource is INotifyCollectionChanged)
            {
                //auto-scroll on ItemsSource change
                ((INotifyCollectionChanged) listView.ItemsSource).CollectionChanged += (sender, args) =>
                {
                    var lastItem = args.NewItems.OfType<object>().LastOrDefault();
                    if(lastItem != null)
                        Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => listView.ScrollIntoView(lastItem));
                };
            }
        }
    }
}