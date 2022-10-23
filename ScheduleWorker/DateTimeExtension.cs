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

        }
        public static DateOnly GetWeekStart(this DateOnly date) 
        {
            int dayDifference = 0;
            int dateDayOfWeek = (int)date.DayOfWeek;

            if (dateDayOfWeek < 1) { dayDifference = 1; }
            else if (dateDayOfWeek == 1) { dayDifference = 0; }
            else { dayDifference = 1 - dateDayOfWeek; };

            var weekStartDate = date.AddDays(dayDifference);

            return weekStartDate;

        }

        public static DateOnly GetWeekEnd(this DateOnly date) 
        {
            int dayDifference = 0;
            int dateDayOfWeek = (int)date.DayOfWeek;

            if (dateDayOfWeek < 1) { dayDifference = 0; }
            else { dayDifference = 7 - dateDayOfWeek; };

            var weekEndDate = date.AddDays(dayDifference);

            return weekEndDate;

        }

        public static DateOnly GetDateByDayOfWeek(this DateOnly date, string ruDayOfWeekName) 
        {
            var ruDayOfWeek = ruDayOfWeekName.ToLower().Replace(" ", "");
            int inputDayOfWeek = 10;

            if (ruDayOfWeek == "понедельник")
                inputDayOfWeek = 1;
            if (ruDayOfWeek == "вторник")
                inputDayOfWeek = 2;
            if (ruDayOfWeek == "среда")
                inputDayOfWeek = 3;
            if (ruDayOfWeek == "четверг")
                inputDayOfWeek = 4;
            if (ruDayOfWeek == "пятница")
                inputDayOfWeek = 5;
            if (ruDayOfWeek == "суббота")
                inputDayOfWeek = 6;
            if (ruDayOfWeek == "воскресенье")
                inputDayOfWeek = 0;

            if (inputDayOfWeek == 10)
                throw new Exception("input string is invalid");

            int targetDayOfWeek = (int)date.DayOfWeek;

            DateOnly outputDate = date.AddDays(inputDayOfWeek - targetDayOfWeek);

            return outputDate;

        }
    }
}
