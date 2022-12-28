namespace ImportantBot.Data
{
    public class MessageModel

    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}