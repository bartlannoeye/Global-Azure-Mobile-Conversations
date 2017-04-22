using System;
using FormsChatInterface.Services;

namespace ShoppingForms.Models
{
    public class BotMessage
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public DateTime Created { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public string ChannelData { get; set; }
        public string[] Images { get; set; }
        public Attachment[] Attachments { get; set; }
        public string ETag { get; set; }

        public string AuthorName => IsMine() ? Constants.MyName : Constants.FriendName;

        public bool IsMine()
        {
            return Constants.MyUserId.Equals(From);
        }
    }
}
