using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using FormsChatInterface.Services;
using Prism.Commands;
using ShoppingForms.Models;

namespace ShoppingForms.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private readonly BotClient _botClient;

        public MainPageViewModel(BotClient botClient)
        {
            _botClient = botClient;

            SendMessageCommand = new DelegateCommand(OnSendMessage);
        }

        private ObservableCollection<BotMessage> _messages;
        public ObservableCollection<BotMessage> Messages
        {
            get { return _messages; }
            set { SetProperty(ref _messages, value); }
        }

        private string _newText;
        public string NewText
        {
            get { return _newText; }
            set { SetProperty(ref _newText, value); }
        }

        public DelegateCommand SendMessageCommand { get; private set; }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            Messages = new ObservableCollection<BotMessage>();
            var message = await _botClient.CreateConversationAsync();
            Messages.Add(message);
        }

        private async void OnSendMessage()
        {
            MessageSet set = await _botClient.SendMessageAsync(NewText);
            foreach (var botMessage in set.Messages)
            {
                if(!Messages.Any(m => m.Id == botMessage.Id))
                    Messages.Add(botMessage);
            }
        }
    }
}
