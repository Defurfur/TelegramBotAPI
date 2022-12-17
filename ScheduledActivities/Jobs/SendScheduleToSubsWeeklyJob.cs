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
using TelegramBotService.Abstractions;
using Telegram.Bot.Types;
using ReaSchedule.Models;
using User = ReaSchedule.Models.User;

namespace ScheduledActivities.Jobs;

public class SendScheduleToSubsWeeklyJob : IInvocable
{
    private readonly ScheduleDbContext _context;
    private readonly IScheduleLoader _loader;
    private readonly IMessageSender _sender;
    private readonly ILogger<SendScheduleToSubsWeeklyJob> _logger;
    private readonly TimeOfDay _timeOfDay;
    private List<User>? _users;
    public SendScheduleToSubsWeeklyJob(
        ScheduleDbContext context,
        ILogger<SendScheduleToSubsWeeklyJob> logger,
        IScheduleLoader loader,
        IMessageSender sender,
        TimeOfDay timeOfDay)
    {
        _context = context;
        _logger = logger;
        _loader = loader;
        _sender = sender;
        _timeOfDay = timeOfDay;
    }


    public async Task Invoke()
    {
        _logger.LogInformation("{Task} with param 'timeofDay' = '{timeOfDay}' has started",
            GetType().Name,
            _timeOfDay.Humanize());

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await Process();
        }
        catch(Exception ex)
        {
            _logger.LogInformation(ex, "{exception} was thrown", ex.GetType().Name);
        }
        finally
        {
            stopwatch.Stop();
            var userAmount = _users is null 
                ? 0 
                : _users.Count;

            _users = null;

            _logger.LogInformation("[Metrics] {Task} with params " +
                "'timeofDay' = '{timeOfDay}'  took {Time} to finish. {UserAmount} " +
                "users have recieved schedule",
            GetType().Name,
            _timeOfDay.Humanize(),
            stopwatch.Elapsed.Humanize(2),
            userAmount);

        }
              
    }

    private async Task Process()
    {
        _users = await _context
            .Users
            .Include(x => x.SubscriptionSettings)
            .Where(x =>
                x.SubscriptionSettings != null
                && x.SubscriptionSettings.UpdateSchedule == UpdateSchedule.EveryWeek
                && x.SubscriptionSettings.DayAmountToUpdate == DayAmountToUpdate.NotSet
                && x.SubscriptionSettings.TimeOfDay != TimeOfDay.NotSet
                && x.SubscriptionSettings.WeekToSend != WeekToSend.NotSet
                && (int)x.SubscriptionSettings.DayOfUpdate! == (int)DateTime.Now.DayOfWeek
                )
            .AsSplitQuery()
            .ToListAsync();

        if (_users is null)
            return;

        var tasks = _users
            .Select(x => FormatAndSendSchedule(
                x,
                x.SubscriptionSettings!.WeekToSend));

        await Task.WhenAll(tasks);

    }

    private async Task FormatAndSendSchedule(
        ReaSchedule.Models.User user,
        WeekToSend weekToSend)
    {
        if (weekToSend == WeekToSend.NotSet)
            return;

        var weekIndex = weekToSend == WeekToSend.CurrentWeek 
            ? 0
            : 1;

        var formattedText = await _loader.DownloadFormattedScheduleAsync(user, weekIndex);

        await _sender.SendMessageWithSomeText(user.ChatId, formattedText);

    }
}
