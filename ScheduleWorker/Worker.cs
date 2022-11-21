using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduleUpdateService.Services;
using XAct;

namespace ScheduleWorker;



public class Worker : BackgroundService

{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ScheduleDbContext _context;
    private readonly IParserPipeline _parserPipeline;

    public Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory,
        ScheduleDbContext context,
        IParserPipeline parserPipeline)

    {

        _logger = logger;
        _scopeFactory = scopeFactory;
        _context = context;
        _parserPipeline = parserPipeline;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)

    {

        using var scope = _scopeFactory.CreateScope();

        var hostLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

        try
        {
            await Parse(stoppingToken);
        }

        catch (TaskCanceledException tce)
        {
            _logger.LogWarning(tce, "Task was cancelled by user or system");
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected end of execution");
        }

        hostLifetime!.StopApplication();

    }

    private async Task Parse(CancellationToken ct)

    {
        var reaGroupList = await _context
            .ReaGroups
            .Include(x => x.ScheduleWeeks!)
                .ThenInclude(x => x.ScheduleDays)
                    .ThenInclude(x => x.ReaClasses)
                    .AsSplitQuery()
                    .ToListAsync();

        var tasks = reaGroupList
            .Select(x => _parserPipeline.ParseAndUpdate(x));
        var results = await Task.WhenAll(tasks);

        var joinedGroups = reaGroupList.Join(
            results, 
            x => x.Id,
            y => y.Id,
            (x,y) => (x,y));

        foreach (var (oldG, newG) in joinedGroups)
        {
            if (oldG.Hash != newG.Hash)
            {
                oldG.ScheduleWeeks = newG.ScheduleWeeks;
                oldG.Hash = newG.Hash;
            }
            
        }

        await _context.SaveChangesAsync(ct);
    }

}