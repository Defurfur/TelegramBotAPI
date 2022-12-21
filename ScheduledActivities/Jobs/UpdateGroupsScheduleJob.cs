using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReaSchedule.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;
using ScheduleUpdateService.Abstractions;
using System.Diagnostics;
using Humanizer;
using System.Diagnostics.Metrics;

namespace ScheduledActivities.Jobs;

public class UpdateGroupsScheduleJob : IInvocable
{
    private readonly IParserPipeline _parserPipeline;
    private readonly ScheduleDbContext _context;
    private readonly ILogger<UpdateGroupsScheduleJob> _logger;

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
        int updatedGroupCounter = 0;

        try
        {
           updatedGroupCounter = await Process();
        }        
        catch(Exception ex)
        {
            _logger.LogInformation(
                ex,
                "{ExceptionName} has been thrown during {ClassType} execution",
                ex.GetType().Name,
                GetType().Name
                );
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("{UpdatedGroupsNumber} groups' schedules have changed" +
                " and been updated in the database. Task took {Time} to finish",
                updatedGroupCounter,
                stopwatch.Elapsed.Humanize(2));
        }


    }

    private async Task<int> Process()
    {
        int counter = 0;
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


        foreach (var (oldG, newG) in joinedGroups)
        {
            if (oldG.Hash != newG.Hash)
            {
                oldG.ScheduleWeeks = newG.ScheduleWeeks;
                oldG.Hash = newG.Hash;
                counter++;
            }
        }

        await _context.SaveChangesAsync();
        return counter;
    }
}
