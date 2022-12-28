using ImportantBot.Core.Constants;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ImportantBot.Core
{
    public class UserCommandHandler
    {
        private TelegramBotClient _botClient;

        public UserCommandHandler()
        {
            var token = Environment.GetEnvironmentVariable("TelegramBotClient", EnvironmentVariableTarget.Process)
                ?? throw new ArgumentException("Токен не задан в переменную окружения");
            _botClient = new TelegramBotClient(token);
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            try
            {
                if (ImportantCommands.Commands.Any(command => messageText.Contains(command)))
                {
                    //TODO collect messages here
                    var messages = "Some important messages here for the last 24 hours";
                    await _botClient.SendTextMessageAsync(chatId: chatId,
                                                    text: messages,
                                                    replyMarkup: Commands.GetCommandButtons(),
                                                    cancellationToken: cancellationToken);
                    return;
                }

                switch (messageText)
                {
                    case "Важное":
                        var messages = "Some important messages here for the last 24 hours";
                        await _botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: messages,
                                                        replyMarkup: Commands.GetCommandButtons(),
                                                        cancellationToken: cancellationToken);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException)
            {
                var errorMessage = $"Telegram API Error:\n[{((ApiRequestException)exception).ErrorCode}]\n{((ApiRequestException)exception).Message}";
                Console.WriteLine(errorMessage);
                return Task.CompletedTask;
            }
            Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }
    }
}