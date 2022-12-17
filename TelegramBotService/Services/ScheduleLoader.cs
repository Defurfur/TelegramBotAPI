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
    /// Downloads <paramref name="user"/> group's schedule from context and formats it
    /// to a further sending as a telegram message.
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
    /// <summary>
    /// Downloads <paramref name="user"/> group's schedule for <paramref name="dayAmount"/> days from today
    /// and formats it to a further sending as a telegram message.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="dayAmount"></param>
    /// <param name="startWithNextDay"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public async Task<string> DownloadFormattedScheduleNDaysAsync(
        User user,
        int dayAmount,
        bool startWithNextDay)
    {
        var scheduleDays = await _contextUpdateService
            .DownloadNfollowingDaysFromSchedule(user, dayAmount, startWithNextDay);

        ArgumentNullException.ThrowIfNull(scheduleDays, nameof(scheduleDays));

        var formattedSchedule = _scheduleFormatter.Format(scheduleDays);

        var dayAmountLastDigit = dayAmount % 10;

        if (dayAmountLastDigit == 1)
            return $"Расписание на следующий день: \r\n" + formattedSchedule;
        
        bool isFromTwoToFour = dayAmountLastDigit >= 2 && dayAmountLastDigit <= 4;



        formattedSchedule = isFromTwoToFour
            ? $"Расписание на следующие {dayAmount} дня: \r\n" + formattedSchedule
            : $"Расписание на следующие {dayAmount} дней: \r\n" + formattedSchedule;

        return formattedSchedule ?? string.Empty;
    }

    /// <summary>
    /// Downloads <paramref name="user"/> group's schedule from context and formats its
    /// week's schedule defined by zero-based <paramref name="weekIndex"/> to a further
    /// sending as a telegram message. If 
    /// there is no week by <paramref name="weekIndex"/> returns empty string. 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="weekIndex"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public async Task<string> DownloadFormattedScheduleAsync(User user, int weekIndex )
    {
        var group = await _contextUpdateService.DownloadUserScheduleAsync(user);

        ArgumentNullException.ThrowIfNull(group, nameof(group));
        ArgumentNullException.ThrowIfNull(group.ScheduleWeeks, nameof(group.ScheduleWeeks));

        var weekToFormat = group
            .ScheduleWeeks
            .ToList()
            .ElementAtOrDefault(weekIndex);

        var formattedSchedule = weekToFormat != null 
            ? _scheduleFormatter.Format(weekToFormat)
            : string.Empty;




        return formattedSchedule;
    }
    
}
