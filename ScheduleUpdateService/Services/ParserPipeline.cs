using Microsoft.Extensions.Logging;
using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using System.IO;

namespace ScheduleUpdateService.Services;
public class ParserPipeline : IParserPipeline
{
    private readonly IScheduleParser _scheduleLoader;
    private readonly IEntityUpdater _entityUpdater;
    private readonly ILogger<ParserPipeline> _logger;
    public int WeekCountToParse { get; private set; } = 2;

    public ParserPipeline(
        IScheduleParser scheduleLoader,
        IEntityUpdater entituUpdater,
        ILogger<ParserPipeline> logger)
    {
        _scheduleLoader = scheduleLoader;
        _entityUpdater = entituUpdater;
        WeekCountToParse = WeekCountToParse;
        _logger = logger;
    }


    public async Task<ReaGroup> ParseAndUpdate(ReaGroup reaGroup, CancellationToken ct = default)
    {
        if (WeekCountToParse < 1)
            throw new Exception("Cannot parse 0 or less weeks");

        List<WeeklyClassesWrapper> listOfWeekClassWrappers;
        ReaGroup updatedReaGroup = new();

        try
        {
            listOfWeekClassWrappers = await _scheduleLoader
                .LoadPageContentAndParse(WeekCountToParse, reaGroup, ct);

            updatedReaGroup = _entityUpdater
                .UpdateReaGroup(reaGroup, listOfWeekClassWrappers);
        }
        catch(IOException ioException)
        {
            _logger.LogInformation(ioException, "[ParserPipeline] {ExceptionName} has been thrown during task execution",
                ioException.GetType().Name);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[ParserPipeline] {ExceptionName} has been thrown during task execution",
               ex.GetType().Name);
            throw;
        }


        return updatedReaGroup;
    }

    public void SetWeekCount(int weekCount)
    {
        WeekCountToParse = weekCount;
    }
}
