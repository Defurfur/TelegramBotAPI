using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services;

public static class DictionaryStorage
{
    public static Dictionary<string, Action<SubscriptionSettings>> CallbackSettingsActionsDictionary = new()
    {
        {"Enable Subscription", (x) =>  x.EnableSubscription()},
        {"Disable Subscription", (x) =>  x.DisableSubscription()},
        {"Change subscription settings", (x) => x.Reset(true)},

        {"Show Schedule: every day", (x) =>  x.Update(UpdateSchedule.EveryDay)},
        {"Show Schedule: every week", (x) =>  x.Update(UpdateSchedule.EveryWeek)},

        {"Send every day for: 1 day", (x) =>  x.Update(DayAmountToUpdate.OneDay)},
        {"Send every day for: 2 days", (x) =>  x.Update(DayAmountToUpdate.TwoDays)},
        {"Send every day for: 3 days", (x) =>  x.Update(DayAmountToUpdate.ThreeDays)},

        {"Send every week on: Monday", (x) =>  x.Update(DayOfWeekEx.Monday)},
        {"Send every week on: Tuesday", (x) =>  x.Update(DayOfWeekEx.Tuesday)},
        {"Send every week on: Wednesday", (x) =>  x.Update(DayOfWeekEx.Wednesday)},
        {"Send every week on: Thursday", (x) =>  x.Update(DayOfWeekEx.Thursday)},
        {"Send every week on: Friday", (x) =>  x.Update(DayOfWeekEx.Friday)},
        {"Send every week on: Saturday", (x) =>  x.Update(DayOfWeekEx.Saturday)},
        {"Send every week on: Sunday", (x) =>  x.Update(DayOfWeekEx.Sunday)},

        {"WeekToSend: Current", (x) =>  x.Update(WeekToSend.CurrentWeek)},
        {"WeekToSend: Next", (x) =>  x.Update(WeekToSend.NextWeek)},

        {"TimeOfDay: Morning", (x) =>  x.Update(TimeOfDay.Morning)},
        {"TimeOfDay: Afternoon", (x) =>  x.Update(TimeOfDay.Afternoon)},
        {"TimeOfDay: Evening", (x) =>  x.Update(TimeOfDay.Evening)},

        {"IncludeToday: True", (x) => x.Update(true)},
        {"IncludeToday: False", (x) => x.Update(false)},
    };

    public static Dictionary<string, Func<ICallbackMessageUpdater, CallbackQuery, Task<Message>>> MessageUpdaterAndTasksDict = new()
     {
         {"Show Schedule: every day", static (updater, data) => updater.UpdateWithDayNumberOptionsKeyboard(data)},
         {"Show Schedule: every week", static (updater, data) => updater.UpdateWithWeeklyScheduleOptionsKeyboard(data)},
         {"Send every day for", static (updater, data) => updater.UpdateWithTimeOfDayKeyboard(data)},
         {"TimeOfDay", static (updater, data) => updater.UpdateWithIncludeTodayKeyboard(data)},
         {"Change subscription settings", static (updater, data) => updater.UpdateWithScheduleFrequencyOptionsKeyboard(data)},
         {"Send every week on", static (updater, data) => updater.UpdateWithWeekToSendKeyboard(data)},
         {"WeekToSend", static (updater, data) => updater.UpdateWithTimeOfDayKeyboard(data)},
     };

    public static Dictionary<string, string> OrdinalNumberAndEmojiDict = new()
    {
        {"1 пара", "\u0031\u20E3 8:30 - 10:00" },
        {"2 пара", "\u0032\u20E3 10:10 - 11:40" },
        {"3 пара", "\u0033\u20E3 11:50 - 13:20" },
        {"4 пара", "\u0034\u20E3 14:00 - 15:30" },
        {"5 пара", "\u0035\u20E3 15:40 - 17:10" },
        {"6 пара", "\u0036\u20E3 17:20 - 18:50" },
        {"7 пара", "\u0037\u20E3 18:55 - 20:25" },
        {"8 пара", "\u0038\u20E3 20:30 - 22:00" },
    };
}
