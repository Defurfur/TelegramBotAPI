using ReaSchedule.Models;

namespace TelegramBotService.Abstractions;

public interface IScheduleFormatter
{
    string Format(List<ReaClass> reaClasses);
    string Format(ReaGroup reaGroup);
    string Format(ScheduleDay scheduleDay);
    string Format(ScheduleWeek scheduleWeek);
}