using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions;

public interface IParserPipeline
{
    int WeekCountToParse { get; set; }
    Task<ReaGroup> ParseAndUpdate(ReaGroup reaGroup);
}
