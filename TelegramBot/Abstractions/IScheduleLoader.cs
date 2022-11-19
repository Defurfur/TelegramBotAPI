using ReaSchedule.Models;

namespace TelegramBot.Abstractions
{
    public interface IScheduleLoader
    {
        Task<string> DownloadFormattedScheduleAsync(User user);
        Task<ReaGroup> DownloadUserScheduleAsync(User user);
    }
}