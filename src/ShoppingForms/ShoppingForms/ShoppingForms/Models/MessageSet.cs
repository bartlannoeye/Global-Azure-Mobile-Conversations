namespace ShoppingForms.Models
{
    public class MessageSet
    {
        public BotMessage[] Messages { get; set; }
        public string Watermark { get; set; }
        public string ETag { get; set; }
    }
}
