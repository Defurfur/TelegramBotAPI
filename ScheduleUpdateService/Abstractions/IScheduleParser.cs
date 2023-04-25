using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions;

public interface IScheduleParser
{
    Task<bool> CheckForGroupExistance(string groupName, CancellationToken ct = default);
    Task<List<WeeklyClassesWrapper>> LoadPageContentAndParse(
        int weekCountToParse,
        ReaGroup reaGroup,
        CancellationToken ct = default);
}
