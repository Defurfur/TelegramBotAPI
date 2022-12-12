using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.Abstractions
{
    public interface ICallbackMessageUpdater
    {
        Task<Message> UpdateWithCustomTextAndKeyboard(CallbackQuery callback, string text, InlineKeyboardMarkup keyboard);
        Task<Message> UpdateWithDayNumberOptionsKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithErrorMessage(CallbackQuery callback);
        Task<Message> UpdateWithScheduleFrequencyOptionsKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithSubscriptionDisabled(CallbackQuery callback);
        Task<Message> UpdateWithSuccessMessage(CallbackQuery callback);
        Task<Message> UpdateWithWeeklyScheduleOptionsKeyboard(CallbackQuery callback);
    }
}