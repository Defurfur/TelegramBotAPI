using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services;

public class ScheduleLoader : IScheduleLoader
{
    private readonly IScheduleFormatter _scheduleFormatter;
    private readonly IContextUpdateService _contextUpdateService;
    public ScheduleLoader(
        IScheduleFormatter scheduleFormatter,
        IContextUpdateService contextUpdateService)
    {
        _scheduleFormatter = scheduleFormatter;
        _contextUpdateService = contextUpdateService;
    }
    /// <summary>
    /// Downloads <paramref name="user"/> group's schedule from context and formats it to a further send as a telegram message.
    /// </summary>
    /// <param name="user"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public async Task<string> DownloadFormattedScheduleAsync(User user)
    {
        var group = await _contextUpdateService.DownloadUserScheduleAsync(user);
        ArgumentNullException.ThrowIfNull(group, nameof(group));

        var formattedSchedule = _scheduleFormatter.Format(group);

        return formattedSchedule;
    }
    
}
