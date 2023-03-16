using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReaSchedule.DAL;
using Coravel.Invocable;
using ScheduleUpdateService.Abstractions;
using System.Diagnostics;
using Humanizer;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Polly;
using XAct.Resources;
using Polly.Wrap;

namespace ScheduledActivities.Jobs;

public class UpdateGroupsScheduleJob : IInvocable, IAsyncDisposable
{
    private readonly IParserPipeline _parserPipeline;
    private readonly ScheduleDbContext _context;
    private readonly ILogger<UpdateGroupsScheduleJob> _logger;
    private readonly AsyncPolicyWrap _timeoutOnRetryPolicy;

    private bool _isStopped = false;
    private List<string> _updatedGroups = new();
    public CancellationTokenSource _ctSource = new();
    private int _updatedGroupCounter;

    public UpdateGroupsScheduleJob(
        IParserPipeline parserPipeline,
        ScheduleDbContext context,
        ILogger<UpdateGroupsScheduleJob> logger)
    {
        _parserPipeline = parserPipeline;
        _context = context;
        _logger = logger;

        IAsyncPolicy timeoutPerRetryPolicy = Policy
            .TimeoutAsync(TimeSpan.FromMinutes(5));

        IAsyncPolicy retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(1, x => TimeSpan.FromSeconds(30));

        _timeoutOnRetryPolicy = retryPolicy.WrapAsync(timeoutPerRetryPolicy);
    }
    // What we need: 
    // If a task was cancelled - we quit without retry
    // If a task was cancelled due to timeout - we retry once

    public async Task Invoke()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var ct = _ctSource.Token;
            var result = await _timeoutOnRetryPolicy
                .ExecuteAndCaptureAsync(Process, ct);

            if(result.FinalException is not null)
            {
                _logger.LogError(result.FinalException, "{ExceptionName} has been thrown during {ClassType} execution",
                result.FinalException.GetType().Name,
                GetType().Name
                );
            }
        }        
        catch(Exception ex)
        {
            _logger.LogError(
                ex,
                "{ExceptionName} has been thrown during {ClassType} execution",
                ex.GetType().Name,
                GetType().Name
                );
        }
        finally
        {
            _ctSource.Dispose();

            string updatedGroups = 
                _updatedGroups.Count == 0
                ? "none"
                : "\r\n" + string.Join(",\r\n", _updatedGroups);

            stopwatch.Stop();
            _logger.LogInformation("{UpdatedGroupsNumber} groups' schedules have changed" +
                " and been updated in the database. Task took {Time} to finish. \r\n" +
                "Updated groups: {updatedGroups}",
                _updatedGroupCounter,
                stopwatch.Elapsed.Humanize(2),
                updatedGroups);
            _isStopped = true;
        }


    }

    private async Task Process(CancellationToken ct)
    {
        _updatedGroupCounter = 0;
        _updatedGroups.Clear();

        var reaGroupList = await _context
            .ReaGroups
            .Include(x => x.ScheduleWeeks!)
                .ThenInclude(x => x.ScheduleDays)
                    .ThenInclude(x => x.ReaClasses)
                    .AsSplitQuery()
                    .ToListAsync();

        _logger.LogInformation("Received {GroupCount} groups from database." +
            " Starting update process",
            reaGroupList.Count);

        var tasks = reaGroupList
            .Select( x => _parserPipeline.ParseAndUpdate(x, ct));

        var results = await Task.WhenAll(tasks);

        var joinedGroups = reaGroupList.Join(
            results,
            x => x.Id,
            y => y.Id,
            (x, y) => (x, y));


        foreach (var (oldGroup, newGroup) in joinedGroups)
        {
            if (newGroup.ScheduleWeeks is null)
                continue;

            if (oldGroup.Hash != newGroup.Hash)
            {
                oldGroup.ScheduleWeeks = newGroup.ScheduleWeeks;
                oldGroup.Hash = newGroup.Hash;
                _updatedGroups.Add(oldGroup.GroupName);
                _updatedGroupCounter++;
            }
        }
        
        await _context.SaveChangesAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        int disposeTimeoutCounter = 50;

        if(!_isStopped)
            _ctSource.Cancel();

        while(_isStopped != true || disposeTimeoutCounter != 0)
        {
            await Task.Delay(100);
            disposeTimeoutCounter--;
        }

        if(disposeTimeoutCounter == 0)
        {
            _logger.LogWarning("Could not dispose {this} properly", GetType().Name);
        }

        _ctSource.Dispose();
    }
}
