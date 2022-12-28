using ImportantBot.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ImportantBot.Collector
{
    public static class ImportantMessageCollector
    {
        private const string SetUpFunctionName = "setup";
        private const string UpdateFunctionName = "handleupdate";
        private static TelegramBotClient _botClient;

        static ImportantMessageCollector()
        {
            _botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TelegramBotToken", EnvironmentVariableTarget.Process));
        }

        [FunctionName(nameof(ImportantMessageCollector))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Начинаю слушать полученные сообщения.");

            var commandHandler = new RequestHandler(req, log);
            var result = await commandHandler.Handle();

            return new OkObjectResult(result);
        }
    }
}