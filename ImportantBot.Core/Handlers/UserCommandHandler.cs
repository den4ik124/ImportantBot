using ImportantBot.Core.Constants;
using ImportantBot.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ImportantBot.Core
{
    public class UserCommandHandler
    {
        private TelegramBotClient _botClient;
        private DataContext _context;

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
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            try
            {
                if (ImportantCommands.Commands.Any(command => messageText.Contains(command)) && messageText.Length > 1)
                {
                    //TODO collect messages here
                    var message = new MessageModel()
                    {
                        UserId = update.Message.From.Id,
                        UserName = $"{update.Message.From.FirstName} {update.Message.From.LastName}",
                        Link = "",
                        Text = messageText
                    };
                    await _context.InsertData(message);
                    //await AddMessageToDatabase(update.Message);
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

        //private async Task AddMessageToDatabase(Message message)
        //{
        //    var connectionString = new SqlConnectionStringBuilder()
        //    {
        //        DataSource = @"(localdb)\MSSQLLocalDB",
        //        InitialCatalog = "ChatMessages",
        //        IntegratedSecurity = true,
        //    };

        //    var sqlQuery = "INSERT INTO [ChatMessages].[dbo].[ChatMessages] ([UsedId], [UserName], [MessageLink], [MessageText])\r\n  VALUES (@userId, @userName, @messageLink, @messageText) ";

        //    using (var connection = new SqlConnection(connectionString.ConnectionString))
        //    {
        //        await connection.OpenAsync();
        //        var sqlCommand = new SqlCommand(sqlQuery, connection);
        //        var parameterId = new SqlParameter("@userId", message.From.Id);
        //        var parameterUserName = new SqlParameter("@userName", message.From.FirstName + message.From.LastName);
        //        var parameterMessageLink = new SqlParameter("@messageLink", "");
        //        var parameterMessage = new SqlParameter("@messageText", message.Text);

        //        sqlCommand.Parameters.AddRange(new[] { parameterId, parameterUserName, parameterMessageLink, parameterMessage });

        //        var numberOfNotes = await sqlCommand.ExecuteNonQueryAsync();
        //    }
        //}

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