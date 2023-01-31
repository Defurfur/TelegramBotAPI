using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions;

public interface IParserPipeline
{
    int WeekCountToParse { get; }
    Task<ReaGroup> ParseAndUpdate(ReaGroup reaGroup);
    void SetWeekCount(int weekCount);
}
