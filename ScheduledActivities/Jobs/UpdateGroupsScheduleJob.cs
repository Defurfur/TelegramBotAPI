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
            .Select(x => _parserPipeline.ParseAndUpdate(x));
        var results = await Task.WhenAll(tasks);

        var joinedGroups = reaGroupList.Join(
            results,
            x => x.Id,
            y => y.Id,
            (x, y) => (x, y));

        int updatedGroupCounter = 0;

        foreach (var (oldG, newG) in joinedGroups)
        {
            if (oldG.Hash != newG.Hash)
            {
                oldG.ScheduleWeeks = newG.ScheduleWeeks;
                oldG.Hash = newG.Hash;
                updatedGroupCounter++;
            }

        }

        await _context.SaveChangesAsync();
        stopwatch.Stop();
        _logger.LogInformation("{UpdatedGroupsNumber} groups' schedules have changed" +
            " and been updated in the database. Task took {Time} to finish",
            updatedGroupCounter,
            stopwatch.Elapsed.Humanize(2));


    }
}
