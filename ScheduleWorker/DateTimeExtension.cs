using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleWorker
{
    public static class DateTimeExtension
    {
        public static int GetWeekNumber(this DateTime dateTime)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly firstOfSeptember = new DateOnly(today.Year, 9, 1);

            var currentWeekStart = GetWeekStart(today);
            var firstWeekStart = GetWeekStart(firstOfSeptember);

            int weekNumber = (currentWeekStart.Day - firstWeekStart.Day - 1)/7 + 1;

            return weekNumber;

            static DateOnly GetWeekStart(DateOnly date) 
            {
                int dayDifference = 0;
                int dateDayOfWeek = (int)date.DayOfWeek;

                if (dateDayOfWeek < 1) { dayDifference = 1; }
                else if (dateDayOfWeek == 1) { dayDifference = 0; }
                else { dayDifference = 1 - dateDayOfWeek; };

                var weekStartDate = date.AddDays(dayDifference);

                return weekStartDate;

            }
        }
    }
}
