using ReaSchedule.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.Abstractions
{
    public interface ICallbackMessageUpdater
    {
        Task<Message> UpdateWithCustomTextAndKeyboard(CallbackQuery callback, string text, InlineKeyboardMarkup keyboard);
        Task<Message> UpdateWithDayNumberOptionsKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithErrorMessage(CallbackQuery callback);
        Task<Message> UpdateWithIncludeTodayKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithScheduleFrequencyOptionsKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithSubscriptionKeyboard(CallbackQuery callback, SubscriptionSettings settings, bool subscriptionEnabled);
        Task<Message> UpdateWithSuccessMessage(SubscriptionSettings settings, CallbackQuery callback);
        Task<Message> UpdateWithTimeOfDayKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithWeeklyScheduleOptionsKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithWeekToSendKeyboard(CallbackQuery callback);
    }
}