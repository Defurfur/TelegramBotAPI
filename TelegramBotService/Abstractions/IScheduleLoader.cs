using ReaSchedule.Models;

namespace TelegramBotService.Abstractions
{
    public interface IScheduleLoader
    {
        Task<string> DownloadFormattedScheduleAsync(User user);
    }
}