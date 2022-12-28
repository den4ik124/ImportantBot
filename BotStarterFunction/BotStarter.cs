using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BotStarterFunction
{
    public static class BotStarter
    {
        [FunctionName(nameof(BotStarter))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var token = Environment.GetEnvironmentVariable("TelegramBotToken", EnvironmentVariableTarget.Process);
            if (token is null)
            {
                return new NotFoundObjectResult("Токен не задан.");
            }
            var botClient = new TelegramBotClient(token);

            var message = "Fuckup произошел";
            try
            {
                await botClient.SetWebhookAsync("https://importantbotcollector.azurewebsites.net/api/ImportantMessageCollector");

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }
        }
    }
}