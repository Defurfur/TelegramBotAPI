using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions;

public interface IScheduleParser
{
    Task<bool> CheckForGroupExistance(string groupName);
    Task<List<WeeklyClassesWrapper>> LoadPageContentAndParse(
        int weekCountToParse,
        ReaGroup reaGroup);
}
