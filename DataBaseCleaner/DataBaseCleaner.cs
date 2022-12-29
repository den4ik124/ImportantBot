using ImportantBot.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DataBaseCleaner
{
    public class DataBaseCleaner
    {
        [FunctionName(nameof(DataBaseCleaner))]
        public async Task Run([TimerTrigger("0 0 0 * * Mon")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var connectionString = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process)
                     ?? throw new ArgumentException("Строка подключения не задана в переменную окружения");

            var context = new DataContext(connectionString);
            await context.ClearTable();

            var token = Environment.GetEnvironmentVariable("TelegramBotToken", EnvironmentVariableTarget.Process)
                    ?? throw new ArgumentException("Токен не задан в переменную окружения");

            var botClient = new TelegramBotClient(token);

            await botClient.SendTextMessageAsync(chatId: 739383661,
                                text: "База данных была обновлена. Важные сообщения удалены.",
                                cancellationToken: CancellationToken.None);
        }
    }
}