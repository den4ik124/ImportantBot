using ImportantBot.Core.Constants;

namespace ImportantBot.Core.Interfaces
{
    public interface ICommand
    {
        public SenderType Sender { get; }
    }
}