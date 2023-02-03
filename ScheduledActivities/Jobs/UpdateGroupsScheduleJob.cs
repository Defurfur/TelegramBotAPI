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

namespace ScheduledActivities.Jobs;

public class UpdateGroupsScheduleJob : IInvocable
{
    private readonly IParserPipeline _parserPipeline;
    private readonly ScheduleDbContext _context;
    private readonly ILogger<UpdateGroupsScheduleJob> _logger;
    private List<string> _updatedGroups = new();
    private int _updatedGroupCounter;
    public UpdateGroupsScheduleJob(
        IParserPipeline parserPipeline,
        ScheduleDbContext context,
        ILogger<UpdateGroupsScheduleJob> logger)
    {
        _parserPipeline = parserPipeline;
        _context = context;
        _logger = logger;
    }

    public async Task Invoke()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
           await Process();
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
        }


    }

    private async Task Process()
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
            .Select(_parserPipeline.ParseAndUpdate);

        var results = await Task.WhenAll(tasks);

        var joinedGroups = reaGroupList.Join(
            results,
            x => x.Id,
            y => y.Id,
            (x, y) => (x, y));


        foreach (var (oldGroup, newGroup) in joinedGroups)
        {
            
            if (oldGroup.Hash != newGroup.Hash)
            {
                oldGroup.ScheduleWeeks = newGroup.ScheduleWeeks;
                oldGroup.Hash = newGroup.Hash;
                _updatedGroups.Add(oldGroup.GroupName);
                _updatedGroupCounter++;
            }
        }

        await _context.SaveChangesAsync();
    }
}
