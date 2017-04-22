using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShoppingForms.Models;

namespace FormsChatInterface.Services
{
    public class BotClient
    {
        private readonly HttpClient _client;
        private Conversation _lastConversation;

        public BotClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://directline.botframework.com/api/conversations/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", Constants.DirectLineKey);
        }

        public async Task<BotMessage> CreateConversationAsync()
        {
            var response = await _client.GetAsync("/api/tokens/");

            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                HttpContent content = new StringContent(JsonConvert.SerializeObject(conversation), Encoding.UTF8, "application/json");

                response = await _client.PostAsync("/api/conversations/", content);

                if (response.IsSuccessStatusCode)
                {
                    var conversationResponse = await response.Content.ReadAsStringAsync();
                    _lastConversation = JsonConvert.DeserializeObject<Conversation>(conversationResponse);
                }
            }

            return new BotMessage { Text = "Hey"};
        }

        public async Task<MessageSet> SendMessageAsync(string messageText)
        {
            try
            {
                var messageToSend = new BotMessage
                {
                    Text = messageText,
                    ConversationId = _lastConversation.ConversationId,
                    From = Constants.MyUserId
                };

                var content = new StringContent(JsonConvert.SerializeObject(messageToSend), Encoding.UTF8, "application/json");
                var conversationUrl = "https://directline.botframework.com/api/conversations/" + _lastConversation.ConversationId + "/messages/";

                var response = await _client.PostAsync(conversationUrl, content);
                var messageInfo = await response.Content.ReadAsStringAsync(); // possible error which we'll happily ignore for demo purposes

                var messagesRecievedResponse = await _client.GetAsync(conversationUrl);
                var messagesReceivedData = await messagesRecievedResponse.Content.ReadAsStringAsync();
                var messages = JsonConvert.DeserializeObject<MessageSet>(messagesReceivedData);

                return messages;
            }
            catch (Exception)
            {
                // poke catch
                return null;
            }
        }
    }
}
