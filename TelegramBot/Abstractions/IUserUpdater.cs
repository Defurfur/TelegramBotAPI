using ReaSchedule.Models;

namespace TelegramBot.Abstractions
{
    public interface IUserUpdater
    {
        void ProcessCallbackAndSaveChanges(User user, string callbackData);
    }
}