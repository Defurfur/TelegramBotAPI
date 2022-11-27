using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;

namespace ScheduleUpdateService.Services;
public class ParserPipeline : IParserPipeline
{
    private readonly IScheduleParser _scheduleLoader;
    private readonly IEntityUpdater _entityUpdater;
    public int WeekCountToParse { get; set; } = 2;

    public ParserPipeline(
        IScheduleParser scheduleLoader,
        IEntityUpdater entituUpdater)
    {
        _scheduleLoader = scheduleLoader;
        _entityUpdater = entituUpdater;
        WeekCountToParse = WeekCountToParse;
    }


    public async Task<ReaGroup> ParseAndUpdate(ReaGroup reaGroup)
    {
        if (WeekCountToParse < 1)
            throw new Exception("Cannot parse 0 or less weeks");

        var listOfWeekClassWrappers = await _scheduleLoader
            .LoadPageContentAndParse(WeekCountToParse, reaGroup);

        ReaGroup updatedReaGroup = _entityUpdater
            .UpdateReaGroup(reaGroup, listOfWeekClassWrappers);


        return updatedReaGroup;
    }

    public void SetWeekCount(int weekCount)
    {
        WeekCountToParse = weekCount;
    }
}
