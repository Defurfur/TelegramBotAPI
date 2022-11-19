using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Abstractions;

namespace TelegramBot.Services;

public class ScheduleLoader : IScheduleLoader
{
    private readonly ScheduleDbContext _context;
    private readonly IScheduleFormatter _scheduleFormatter;

    public ScheduleLoader(
        ScheduleDbContext context,
        IScheduleFormatter scheduleFormatter)
    {
        _context = context;
        _scheduleFormatter = scheduleFormatter;
    }

    public async Task<string> DownloadFormattedScheduleAsync(User user)
    {
        var group = await DownloadUserScheduleAsync(user);

        var formattedSchedule = _scheduleFormatter.Format(group);

        return formattedSchedule;
    }
    public async Task<ReaGroup> DownloadUserScheduleAsync(User user)
    {
        var reaGroup = await _context
            .ReaGroups
            .Include(x => x.ScheduleWeeks!)
                .ThenInclude(x => x.ScheduleDays)
                    .ThenInclude(x => x.ReaClasses)
                    .FirstAsync(x => x.Id == user.ReaGroupId);


        return reaGroup;
    }
}
