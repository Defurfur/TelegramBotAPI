using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Abstractions
{
    public interface ICustomKeyboardManager
    {
        InlineKeyboardMarkup? GetNextKeyboard(string? callbackData);
    }
}
