using ImportantBot.Core.Constants;

namespace ImportantBot.Core
{
    public class RequestDataDto
    {
        public SenderType Sender { get; set; }

        //public string Command { get; set; }
        //public string RequestMessage { get; set; }
        public object RequestObject { get; set; }
    }
}