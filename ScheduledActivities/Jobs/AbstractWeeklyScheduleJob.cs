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
    protected abstract ScheduleDbContext Context { get; set; }
    protected abstract IScheduleLoader Loader { get; set; }
    protected abstract IMessageSender Sender { get; set; }
    protected abstract ILogger<AbstractWeeklyScheduleJob> Logger { get; set; }
    protected abstract TimeOfDay TimeOfDay { get; set; }
    protected abstract List<User>? Users { get; set; }


    public async Task Invoke()
    {
        Logger.LogDebug("{Task} with param 'timeofDay' = '{timeOfDay}' has started",
            GetType().Name,
            TimeOfDay.Humanize());

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await Process();
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "{exception} was thrown", ex.GetType().Name);
        }
        finally
        {
            stopwatch.Stop();

            var loggingString = Users is null
                ? "No users, whos settings satisfy task conditions have been found"
                : $"{Users.Count} users have recieved schedule";

            Users = null;

            Logger.LogInformation(
                "[Metrics] {Task} with params " +
                "'timeofDay' = '{timeOfDay}'  took {elapsedTime} to finish. {UserAmuntString}",
                GetType().Name,
                TimeOfDay.Humanize(),
                stopwatch.Elapsed.Humanize(2),
                loggingString);

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
        if (weekToSend == WeekToSend.NotSet)
            return;

        var weekIndex = weekToSend == WeekToSend.CurrentWeek 
            ? 0
            : 1;

        var formattedText = await Loader.DownloadFormattedScheduleAsync(user, weekIndex);

        await Sender.SendMessageWithSomeText(user.ChatId, formattedText);

    }
}
