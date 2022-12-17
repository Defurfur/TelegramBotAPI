using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IUserSettingsFormatter
    {
        string Format(SubscriptionSettings settings);
    }
}