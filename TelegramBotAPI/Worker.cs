using ReaSchedule.DAL;

namespace DataTransferService;

public class DataTransferWorker : BackgroundService
{
    private ScheduleDbContext _oldContext;
    private NewContext _newContext;
    private readonly ILogger<DataTransferWorker> _logger;
    private bool stopp = false;
    public DataTransferWorker(ScheduleDbContext oldContext, NewContext newContext, ILogger<DataTransferWorker> logger)
    {
        _oldContext = oldContext;
        _newContext = newContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Started execution");
        while (stopp == false)
        {
            await _newContext.ReaClasses.AddRangeAsync(_oldContext.ReaClasses.ToList());
            await _newContext.ScheduleDays.AddRangeAsync(_oldContext.ScheduleDays.ToList());
            await _newContext.ScheduleWeeks.AddRangeAsync(_oldContext.ScheduleWeeks.ToList());
            await _newContext.ReaGroups.AddRangeAsync(_oldContext.ReaGroups.ToList());
            await _newContext.Settings.AddRangeAsync(_oldContext.Settings.ToList());

            await _newContext.SaveChangesAsync();

            stopp = true;
            _logger.LogWarning("Ended execution");
        }
    }
}