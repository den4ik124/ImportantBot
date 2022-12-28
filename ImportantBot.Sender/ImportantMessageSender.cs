using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ImportantBot.Sender
{
    public static class ImportantMessageSender
    {
        private static TelegramBotClient _botClient;

        static ImportantMessageSender()
        {
            _botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TelegramBotToken", EnvironmentVariableTarget.Process));
        }

        [FunctionName(nameof(ImportantMessageSender))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var messages = "Получили сообщения из БД и отправили их вам.";
            return new OkObjectResult(nameof(ImportantMessageSender) + $"\n{messages}");
            //            return new OkObjectResult(responseMessage);
        }
    }
}