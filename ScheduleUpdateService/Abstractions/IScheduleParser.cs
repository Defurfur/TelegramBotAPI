using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions;

public interface IScheduleParser
{
    Task<List<WeeklyClassesWrapper>> LoadPageContentAndParse(
        int weekCountToParse,
        ReaGroup reaGroup);
}
