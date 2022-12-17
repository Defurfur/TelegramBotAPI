using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IScheduleLoader
    {
        Task<string> DownloadFormattedScheduleAsync(User user);
        Task<string> DownloadFormattedScheduleAsync(User user, int weekIndex);
        Task<string> DownloadFormattedScheduleNDaysAsync(User user, int dayAmount, bool startWithNextDay);
    }
}