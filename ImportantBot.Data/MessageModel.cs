namespace ImportantBot.Data
{
    public class MessageModel

    {
        private string _messageLinkBase = "https://t.me/c";

        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string MessageLink { get => $"{_messageLinkBase}/{string.Concat(ChatId.ToString().Skip(4))}/{Link}"; }

        public override string ToString()
        {
            return $"@{UserName}:\n{string.Concat(Text.Take(30))}...\n{MessageLink}";
        }
    }
}