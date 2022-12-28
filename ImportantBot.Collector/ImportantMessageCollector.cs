using ImportantBot.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ImportantBot.Collector
{
    public static class ImportantMessageCollector
    {
        [FunctionName(nameof(ImportantMessageCollector))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Начинаю слушать полученные сообщения.");

            var commandHandler = new RequestHandler(req);
            var result = await commandHandler.Handle();

            return new OkObjectResult(result);
        }
    }
}