﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReaSchedule.DAL;
using Coravel.Invocable;
using System.Diagnostics;
using Humanizer;
using TelegramBotService.Abstractions;
using ReaSchedule.Models;
using User = ReaSchedule.Models.User;

namespace ScheduledActivities.Jobs;

public abstract class AbstractDailyScheduleJob : IInvocable
{
    protected abstract ScheduleDbContext Context { get; set; }
    protected abstract IScheduleLoader Loader { get; set; }
    protected abstract IMessageSender Sender { get; set; }
    protected abstract ILogger<AbstractDailyScheduleJob> Logger { get; set; }
    protected abstract TimeOfDay TimeOfDay { get; set; }
    protected abstract List<User>? Users { get; set; }
    //public AbstractDailyScheduleJob(
    //    ScheduleDbContext context,
    //    ILogger<AbstractDailyScheduleJob> logger,
    //    IScheduleLoader loader,
    //    IMessageSender sender)
    //{
    //    Context = context;
    //    Logger = logger;
    //    Loader = loader;
    //    Sender = sender;
    //}

    public async Task Invoke()
    {
        if (TimeOfDay == TimeOfDay.NotSet)
            throw new Exception("Task not configured");

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

        Users = await Context
            .Users
            .Include(x => x.SubscriptionSettings)
            .Where(x =>
                x.SubscriptionSettings != null
                && x.SubscriptionSettings.SubscriptionEnabled == true
                && x.SubscriptionSettings.UpdateSchedule == UpdateSchedule.EveryDay
                && x.SubscriptionSettings.DayAmountToUpdate != DayAmountToUpdate.NotSet
                && x.SubscriptionSettings.DayOfUpdate != DayOfWeekEx.NotSet
                && x.SubscriptionSettings.WeekToSend == WeekToSend.NotSet
                && x.SubscriptionSettings.TimeOfDay == TimeOfDay)
            .AsSplitQuery()
            .ToListAsync();

         if (Users is null)
            return;

        var tasks = Users
            .Select(user => FormatAndSendSchedule(
                user: user,
                dayAmount: (int)user.SubscriptionSettings!.DayAmountToUpdate!,
                startWithNextDay: !user.SubscriptionSettings.IncludeToday));

        await Task.WhenAll(tasks);

    }

    private protected async Task FormatAndSendSchedule(
        ReaSchedule.Models.User user,
        int dayAmount,
        bool startWithNextDay)
    {
        var formattedText = await Loader.DownloadFormattedScheduleNDaysAsync(user, dayAmount, startWithNextDay);

        await Sender.SendMessageWithSomeText(user.ChatId, formattedText);

    }

}
