using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReaSchedule.DAL;
using Coravel.Invocable;
using System.Diagnostics;
using Humanizer;
using TelegramBotService.Abstractions;
using ReaSchedule.Models;
using User = ReaSchedule.Models.User;

namespace ScheduledActivities.Jobs;

public class SendScheduleToSubsDailyJob : IInvocable
{
    private readonly ScheduleDbContext _context;
    private readonly IScheduleLoader _loader;
    private readonly IMessageSender _sender;
    private readonly ILogger<SendScheduleToSubsDailyJob> _logger;
    private readonly TimeOfDay _timeOfDay;
    private List<User>? _users;
    public SendScheduleToSubsDailyJob(
        ScheduleDbContext context,
        ILogger<SendScheduleToSubsDailyJob> logger,
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

            _logger.LogInformation("[Metrics] {Task} with param " +
                "'timeofDay' = '{timeOfDay}' took {Time} to finish. {UserAmount} " +
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
                && x.SubscriptionSettings.SubscriptionEnabled == true
                && x.SubscriptionSettings.UpdateSchedule == UpdateSchedule.EveryDay
                && x.SubscriptionSettings.DayAmountToUpdate != DayAmountToUpdate.NotSet
                && x.SubscriptionSettings.DayOfUpdate == null
                && x.SubscriptionSettings.WeekToSend == WeekToSend.NotSet
                && x.SubscriptionSettings.TimeOfDay == _timeOfDay)
            .AsSplitQuery()
            .ToListAsync();

         if (_users is null)
            return;

        var tasks = _users
            .Select(user => FormatAndSendSchedule(
                user: user,
                dayAmount: (int)user.SubscriptionSettings!.DayAmountToUpdate!,
                startWithNextDay: !user.SubscriptionSettings.IncludeToday));

        await Task.WhenAll(tasks);

    }

    private async Task FormatAndSendSchedule(
        ReaSchedule.Models.User user,
        int dayAmount,
        bool startWithNextDay)
    {
        var formattedText = await _loader.DownloadFormattedScheduleNDaysAsync(user, dayAmount, startWithNextDay);

        await _sender.SendMessageWithSomeText(user.ChatId, formattedText);

    }
}
