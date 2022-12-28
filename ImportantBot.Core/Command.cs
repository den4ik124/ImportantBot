using ImportantBot.Core.Constants;
using ImportantBot.Core.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;

namespace ImportantBot.Core
{
    public class Command : ICommand
    {
        private readonly HttpRequestData request;

        public Command(HttpRequestData request)
        {
            this.request = request;
        }

        public SenderType Sender
        {
            get
            {
                return SenderType.User;
            }
        }

        public HttpRequestData Request { get; set; }
    }
}