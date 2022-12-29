namespace ImportantBot.Data
{
    public class MessageModel

    {
        private string _userName = string.Empty;

        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return $"@{UserName}:\n{string.Concat(Text.Take(30))}...";
        }
    }
}