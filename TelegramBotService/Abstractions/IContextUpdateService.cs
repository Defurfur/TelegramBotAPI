using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IContextUpdateService
    {
        Task AddBug(long chatId, string text, int userId = 0);
        Task<ReaGroup> CreateNewReaGroup(string groupName);
        Task<List<ScheduleDay>> DownloadNfollowingDaysFromSchedule(User user, int dayAmount, bool startWithNextDay);
        Task<ReaGroup?> DownloadUserScheduleAsync(User user);
        Task TryChangeUsersGroupAsync(User user, ReaGroup reaGroup);
        Task TryChangeUsersGroupAsync(User user, string groupName);
        bool TryFindGroupInDb(string text, out ReaGroup? reaGroup);
        Task TryRegisterUserAsync(ReaGroup group, long chatId);
        Task TryRegisterUserAsync(string groupName, long chatId);
        Task UpdateReaGroup(ReaGroup group, List<ScheduleWeek> scheduleWeeks, string hash);
    }
}