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
using XAct;

namespace ScheduledActivities.Jobs;

public abstract class AbstractWeeklyScheduleJob : IInvocable
{
    protected abstract Exception? Exception { get; set; }
    protected abstract ScheduleDbContext Context { get; set; }
    protected abstract IScheduleLoader Loader { get; set; }
    protected abstract IMessageSender Sender { get; set; }
    protected abstract ILogger<AbstractWeeklyScheduleJob> Logger { get; set; }
    protected abstract TimeOfDay TimeOfDay { get; set; }
    protected abstract List<User>? Users { get; set; }


    public async Task Invoke()
    {
        if (TimeOfDay == TimeOfDay.NotSet)
        {
            var exception = new Exception($"[{GetType().Name}] Task not configured. TimeOfDay parameter is set to 'NotSet'");

            await SendTaskResult(GetType().Name, DateTime.Now, exception: exception);

            throw exception;
        }

        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Logger.LogDebug("[{Task}] ScheduledTask with param 'timeofDay' = '{timeOfDay}' has started",
                GetType().Name,
                TimeOfDay.Humanize());
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await Process();
        }
        catch(Exception ex)
        {
            Logger.LogError(
                ex,
                "[{This}] {Exception} was thrown during execution of a scheduled task",
                GetType().Name,
                ex.GetType().Name);

            Exception = ex;
        }
        finally
        {
            stopwatch.Stop();

            var userInfoString = Users is null || !Users.Any()
                ? "No users, whos settings satisfy task conditions have been found"
                : $"{Users.Count} users have recieved schedule";

            var log = $"[{GetType().Name}] ScheduledTask with " +
                    $"TimeOfDay: {TimeOfDay.Humanize()}'  took {stopwatch.Elapsed.Humanize()} to finish."
                    + userInfoString;

            await SendTaskResult(
                GetType().Name,
                DateTime.Now,
                Users?.Count ?? 0,
                log,
                Exception,
                (int)stopwatch.ElapsedMilliseconds);

            Users = null;

            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("[{This}] ScheduledTask with " +
                    "TimeOfDay: {TimeOfDay}'  took {Elapsed} to finish. {UserInfoString}",
                    GetType().Name,
                    TimeOfDay.Humanize(),
                    stopwatch.Elapsed.Humanize(),
                    userInfoString);
            }

        }
              
    }

    private protected async Task Process()
    {
        var todayAsInt = (int)DateTime.Now.DayOfWeek;

        Users = await Context
            .Users
            .Include(x => x.SubscriptionSettings)
            .Where(x =>
                x.SubscriptionSettings != null
                && x.SubscriptionSettings.UpdateSchedule == UpdateSchedule.EveryWeek
                && x.SubscriptionSettings.DayAmountToUpdate == DayAmountToUpdate.NotSet
                && x.SubscriptionSettings.TimeOfDay != TimeOfDay.NotSet
                && x.SubscriptionSettings.WeekToSend != WeekToSend.NotSet
                && (int)x.SubscriptionSettings.DayOfUpdate == todayAsInt
                )
            .AsSplitQuery()
            .ToListAsync();

        if (Users is null)
            return;

        var tasks = Users
            .Select(x => FormatAndSendSchedule(
                x,
                x.SubscriptionSettings!.WeekToSend));

        await Task.WhenAll(tasks);

    }

    private protected async Task FormatAndSendSchedule(
        ReaSchedule.Models.User user,
        WeekToSend weekToSend)
    {
        var weekIndex = 
            weekToSend == WeekToSend.CurrentWeek 
            ? 0
            : 1;

        var formattedText = await Loader.DownloadFormattedScheduleAsync(user, weekIndex);

        await Sender.SendMessageWithSomeText(user.ChatId, formattedText);

    }

    private protected async Task SendTaskResult(
        string taskName,
        DateTime dateTime,
        int actionNumber = 0,
        string message = "",
        Exception? exception = null,
        int elapsed = 0)
    {
        var scheduledTask = new ScheduledTask(
            taskName,
            dateTime,
            message,
            actionNumber,
            actionNumber,
            exception?.ToString() ?? "",
            elapsed);

        Context
            .ScheduledTasks
            .Add(scheduledTask);

        await Context.SaveChangesAsync();
    }

}
