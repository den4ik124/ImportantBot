using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace ImportantBot.Core
{
    public class RequestHandler
    {
        private readonly HttpRequest _request;

        public RequestHandler(HttpRequest request)
        {
            _request = request;
        }

        public async Task<ResponseDto> Handle()
        {
            try
            {
                var requestAsString = await ReadRequestBody(_request);
                var update = JsonConvert.DeserializeObject<Update>(requestAsString);
                if (update != null)
                {
                    var updateHandler = new UserCommandHandler();
                    await updateHandler.HandleUpdateAsync(update, CancellationToken.None);
                    return new ResponseDto()
                    {
                        IsSuccess = true,
                        Message = "Удачно!\nawait updateHandler.HandleUpdateAsync(update, CancellationToken.None);"
                    };
                }
                return new ResponseDto()
                {
                    IsSuccess = false,
                    Message = "НЕудачно!!!"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        private static async Task<string> ReadRequestBody(HttpRequest request)
        {
            using var sr = new StreamReader(request.Body);
            return await sr.ReadToEndAsync();
        }

        public class ResponseDto
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
        }
    }
}