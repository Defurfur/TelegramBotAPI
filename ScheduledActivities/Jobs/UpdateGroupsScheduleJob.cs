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
        var reaGroupList = await _context
       .ReaGroups
       .Include(x => x.ScheduleWeeks!)
           .ThenInclude(x => x.ScheduleDays)
               .ThenInclude(x => x.ReaClasses)
               .AsSplitQuery()
               .ToListAsync();

        _logger.LogInformation($"Recieved {reaGroupList.Count} groups from database." +
            $" Starting update process");

        var tasks = reaGroupList
            .Select(x => _parserPipeline.ParseAndUpdate(x));
        var results = await Task.WhenAll(tasks);

        _logger.LogInformation("Downloaded current schedules and updated the groups");

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
        _logger.LogInformation($"{updatedGroupCounter} groups' schedules have changed" +
            $" and been updated in the database");

        await _context.SaveChangesAsync();



    }
}
