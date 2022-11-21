using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IUserUpdater
    {
        Task ProcessCallbackAndSaveChanges(User user, string callbackData);
    }
}