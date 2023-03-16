using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions;

public interface IParserPipeline
{
    int WeekCountToParse { get; }
    Task<ReaGroup> ParseAndUpdate(ReaGroup reaGroup, CancellationToken ct = default);
    void SetWeekCount(int weekCount);
}
