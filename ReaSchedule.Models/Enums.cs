using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaSchedule.Models
{
    public enum UpdateSchedule
    {
        [Description("не выставлено")]
        NotSet,
        [Description("каждый день")]
        EveryDay,
        [Description("каждую неделю")]
        EveryWeek,
    }
    public enum DayAmountToUpdate
    {
        [Description("не выставлено")]
        NotSet,
        [Description("один день")]
        OneDay,
        [Description("два дня")]
        TwoDays,
        [Description("три дня")]
        ThreeDays,
    }

    public enum TimeOfDay
    {
        [Description("не выставлено")]
        NotSet,
        [Description("утром (до 8:00)")]
        Morning,
        [Description("днем (до 14:00)")]
        Afternoon,
        [Description("вечером (до 20:00)")]
        Evening
    }

    public enum DayOfWeekEx
    {
        [Description("воскресенье")]
        Sunday = 0,
        [Description("понедельник")]
        Monday = 1,
        [Description("вторник")]
        Tuesday = 2,
        [Description("среда")]
        Wednesday = 3,
        [Description("четверг")]
        Thursday = 4,
        [Description("пятница")]
        Friday = 5,
        [Description("суббота")]
        Saturday = 6
    }

    public enum WeekToSend
    {
        [Description("не выставлено")]
        NotSet,
        [Description("текущую неделю")]
        CurrentWeek,
        [Description("следующую неделю")]
        NextWeek
    }

}

