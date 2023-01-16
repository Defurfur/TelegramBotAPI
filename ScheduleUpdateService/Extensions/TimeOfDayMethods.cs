using Humanizer;
using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdateService.Extensions
{
    public static class TimeOfDayMethods
    {
        public static TimeOfDay GetTimeOfDay(int morning, int day, int evening)
        {
            var now = DateTime.UtcNow;

            var thiryMinutesToNow = now.AddMinutes(-30);
            var thiryMinutesAfterNow = now.AddMinutes(30);

            if (thiryMinutesToNow.CompareTo(DateTime.UtcNow.At(morning)) < 0
                & thiryMinutesAfterNow.CompareTo(DateTime.UtcNow.At(morning)) > 0)
            {
                return TimeOfDay.Morning;
            }

            if (thiryMinutesToNow.CompareTo(DateTime.UtcNow.At(day)) < 0
               & thiryMinutesAfterNow.CompareTo(DateTime.UtcNow.At(day)) > 0)
            {
                return TimeOfDay.Afternoon;
            }

            if (thiryMinutesToNow.CompareTo(DateTime.UtcNow.At(evening)) < 0
               & thiryMinutesAfterNow.CompareTo(DateTime.UtcNow.At(evening)) > 0)
            {
                return TimeOfDay.Evening;
            }

            return TimeOfDay.NotSet;
        }
    }
}
