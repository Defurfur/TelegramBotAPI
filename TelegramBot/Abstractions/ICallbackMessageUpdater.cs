using Telegram.Bot.Types;

namespace TelegramBot.Abstractions
{
    public interface ICallbackMessageUpdater
    {
        Task<Message> UpdateWithDayNumberOptionsKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithScheduleFrequencyOptionsKeyboard(CallbackQuery callback);
        Task<Message> UpdateWithSubscriptionDisabled(CallbackQuery callback);
        Task<Message> UpdateWithSuccessMessage(CallbackQuery callback);
        Task<Message> UpdateWithWeeklyScheduleOptionsKeyboard(CallbackQuery callback);
    }
}