using ImportantBot.Core.Constants;
using ImportantBot.Data;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ImportantBot.Core
{
    public class UserCommandHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly DataContext _context;

        public UserCommandHandler()
        {
            var token = Environment.GetEnvironmentVariable("TelegramBotToken", EnvironmentVariableTarget.Process)
                ?? throw new ArgumentException("Токен не задан в переменную окружения");
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process)
                ?? throw new ArgumentException("Строка подключения не задана в переменную окружения");
            _botClient = new TelegramBotClient(token);
            _context = new DataContext(connectionString);
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            // Only process text messages

            if (update.Type != UpdateType.Message) return;
            if (update.Message.Type != MessageType.Text) return;

            var chatId = update.Message!.Chat.Id;
            var messageText = update.Message.Text;

            try
            {
                if (ImportantCommands.Commands.Any(command => messageText!.Contains(command)) && messageText!.Length > 1)
                {
                    //TODO collect messages here
                    var message = new MessageModel()
                    {
                        UserId = update.Message.From!.Id,
                        FullName = $"{update.Message.From.FirstName} {update.Message.From.LastName}",
                        UserName = update.Message.From.Username ?? string.Empty,
                        Link = update.Message.MessageId.ToString(),
                        Text = messageText,
                        DateTime = update.Message.Date,
                        ChatId = update.Message.Chat.Id
                    };
                    await _context.InsertData(message);
                    return;
                }

                switch (messageText)
                {
                    case ImportantBotConstants.Important:
                    case ImportantBotConstants.ImportantChat:
                    case ImportantBotConstants.ImportantGroup:
                        var messages = await _context.GetMessages<MessageModel>(chatId);
                        var responseText = new StringBuilder();
                        var index = 1;
                        foreach (var message in messages)
                        {
                            responseText.AppendLine($"{index}. {message.ToString()}");
                            index++;
                        }

                        await _botClient.SendTextMessageAsync(chatId: update.Message.From.Id,
                                                                text: responseText.ToString(),
                                                                replyMarkup: Commands.GetCommandButtons(),
                                                                cancellationToken: cancellationToken);
                        await _botClient.DeleteMessageAsync(chatId, update.Message.MessageId, cancellationToken);
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

        public static Task HandlePollingErrorAsync(Exception exception)
        {
            if (exception is ApiRequestException ex)
            {
                var errorMessage = $"Telegram API Error:\n[{ex.ErrorCode}]\n{ex.Message}";
                Console.WriteLine(errorMessage);
                return Task.CompletedTask;
            }
            Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }
    }
}