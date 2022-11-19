﻿using ReaSchedule.DAL;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Abstractions;
using User = ReaSchedule.Models.User;

namespace TelegramBot.Services;

public class UserUpdater : IUserUpdater
{
    private readonly ScheduleDbContext _context;

    public UserUpdater(ScheduleDbContext context)
    {
        _context = context;
    }

    public void ProcessCallbackAndSaveChanges(User user, string callbackData)
    {
        if (callbackData == null || user == null)
            return;


        if (callbackData == "Enable Subscription")
        {
            user.SubscriptionEnabled = true;

            _context.Users.Update(user);
            return;
        }
        if (callbackData == "Disable Subscription")
        {
            user.SubscriptionEnabled = false;
            user.DayNumberToUpdate = null;
            user.DayOfUpdate = null;
            user.UpdateSchedule = null;

            _context.Users.Update(user);
            return;
        }
        if (callbackData == "Disable Subscription")
        {
            user.SubscriptionEnabled = false;
            user.DayNumberToUpdate = null;
            user.DayOfUpdate = null;
            user.UpdateSchedule = null;

            _context.Users.Update(user);
            return;
        }
        var dataSplit = callbackData.Split(": ");

        if (dataSplit.Length == 2)
        {
            user.UpdateSchedule = dataSplit[2] switch
            {
                "every day" => UpdateSchedule.EveryDay,
                "every week" => UpdateSchedule.EveryWeek,
                _ => null
            };
            user.DayNumberToUpdate = dataSplit[2] switch
            {
                "1 day" => DayNumberToUpdate.OneDay,
                "2 days" => DayNumberToUpdate.TwoDays,
                "3 days" => DayNumberToUpdate.ThreeDays,
                _ => null
            };
            user.DayOfUpdate = dataSplit[2] switch
            {
                "Monday" => DayOfWeek.Monday,
                "Tuesday" => DayOfWeek.Tuesday,
                "Wednesday" => DayOfWeek.Wednesday,
                "Thursday" => DayOfWeek.Thursday,
                "Friday" => DayOfWeek.Friday,
                "Saturday" => DayOfWeek.Saturday,
                "Sunday" => DayOfWeek.Sunday,
                _ => null
            };

            _context.Update(user);
            return;
        }

    }
}
