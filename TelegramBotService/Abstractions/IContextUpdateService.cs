using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IContextUpdateService
    {
        Task<ReaGroup> CreateNewReaGroup(string groupName);
        Task<ReaGroup?> DownloadUserScheduleAsync(User user);
        Task TryChangeUsersGroupAsync(User user, ReaGroup reaGroup);
        Task TryChangeUsersGroupAsync(User user, string groupName);
        bool TryFindGroupInDb(string text, out ReaGroup? reaGroup);
        Task TryRegisterUserAsync(ReaGroup group, long chatId);
        Task TryRegisterUserAsync(string groupName, long chatId);
        Task UpdateReaGroup(ReaGroup group, List<ScheduleWeek> scheduleWeeks, string hash);
    }
}