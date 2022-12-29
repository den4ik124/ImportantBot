using ImportantBot.Core.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportantBot.Core;

public static class Commands
{
    public static IReplyMarkup GetCommandButtons()
    {
        var commandButtons = new[]
        {
            new [] {new KeyboardButton(ImportantBotConstants.Important)},
        };

        return new ReplyKeyboardMarkup(commandButtons)
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true,
        };
    }

    public static IReplyMarkup GetInlineCommandButtons()
    {
        var inlineButtonsList = new List<List<InlineKeyboardButton>>();
        var buttonsList = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData(ImportantBotConstants.Important),
        };
        inlineButtonsList.Add(buttonsList);
        return new InlineKeyboardMarkup(inlineButtonsList);
    }
}