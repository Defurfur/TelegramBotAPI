using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IContextUpdateService
    {
        Task<ReaGroup?> DownloadUserScheduleAsync(User user);
        Task ParseScheduleWebsiteAndAddToContextNew(string groupName, Func<ReaGroup, Task<ReaGroup>> parseAndUpdateMethod);
        Task TryChangeUsersGroupAsync(User user, ReaGroup reaGroup);
        Task TryChangeUsersGroupAsync(User user, string groupName);
        bool TryFindGroupInDb(string text, out ReaGroup? reaGroup);
        Task TryRegisterUserAsync(ReaGroup group, long chatId);
        Task TryRegisterUserAsync(string groupName, long chatId);
    }
}