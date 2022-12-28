using Telegram.Bot.Types.ReplyMarkups;

namespace ImportantBot.Core
{
    public static class Commands
    {
        public static IReplyMarkup GetCommandButtons()
        {
            var commandButtons = new[]
            {
                new [] {new KeyboardButton("Важные")},
            };

            return new ReplyKeyboardMarkup(commandButtons)
            {
                ResizeKeyboard = true,
            };
        }

        public static IReplyMarkup GetInlineCommandButtons()
        {
            var inlineButtonsList = new List<List<InlineKeyboardButton>>();
            var buttonsList = new List<InlineKeyboardButton>()
            {
                InlineKeyboardButton.WithCallbackData(text: "Важные", callbackData: "Важные"),
            };
            inlineButtonsList.Add(buttonsList);
            return new InlineKeyboardMarkup(inlineButtonsList);
        }
    }
}