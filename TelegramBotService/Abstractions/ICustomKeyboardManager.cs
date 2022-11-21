using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.Abstractions
{
    public interface ICustomKeyboardManager
    {
        InlineKeyboardMarkup? GetNextKeyboard(string? callbackData);
    }
}
