using Telegram.Bot;
using Telegram.Bot.Types;

namespace ImportantBot.Core.Interfaces
{
    public interface IBot
    {
        Task<Message> SendMessageAsync(TelegramBotClient botClient);
    }
}