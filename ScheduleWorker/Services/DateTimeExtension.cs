using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleWorker.Services
{
    public static class DateTimeExtension
    {

        private static readonly Regex _monthRE = new(@"[а-я]+");
        private static readonly Dictionary<string, int> _monthNamesDict = new()
        {
            {"январь",   1 },
            {"февраль",  2 },
            {"март",     3 },
            {"апрель",   4 },
            {"май",      5 },
            {"июнь",     6 },
            {"июль",     7 },
            {"август",   8 },
            {"сентябрь", 9 },
            {"октябрь", 10 },
            {"ноябрь",  11 },
            {"декабрь", 12 },
        };
        public static int GetWeekNumber(this DateTime dateTime)
        {
            DateOnly today = DateOnly.FromDateTime(dateTime);
            return today.GetWeekNumber();

        }

        public static int GetWeekNumber(this DateOnly date)
        {
            DateOnly firstOfSeptember = new(date.Year, 9, 1);

            var currentWeekStart = date.GetWeekStart();
            var firstWeekStart = firstOfSeptember.GetWeekStart();

            TimeSpan timeSpan = currentWeekStart.ToDateTime(new TimeOnly())
                - firstWeekStart.ToDateTime(new TimeOnly());
            int weekNumber = timeSpan.Days / 7 + 1;

            return weekNumber;

        }
        public static DateOnly GetWeekStart(this DateOnly date)
        {
            int dateDayOfWeek = (int)date.DayOfWeek;

            int dayDifference = dateDayOfWeek == 0 ? -6 : 1 - dateDayOfWeek;

            var weekStartDate = date.AddDays(dayDifference);

            return weekStartDate;

        }

        public static DateOnly GetWeekEnd(this DateOnly date)
        {
            int dateDayOfWeek = (int)date.DayOfWeek;

            int dayDifference = dateDayOfWeek == 0 ? 0 : 7 - dateDayOfWeek;

            var weekEndDate = date.AddDays(dayDifference);

            return weekEndDate;

        }

        public static DateOnly GetDateByDayOfWeek(this DateOnly date, string ruDayOfWeekName)
        {

            Dictionary<string, int> DayOfWeekDict = new()
            {
                {"понедельник", 1},
                {"вторник",     2},
                {"среда",       3},
                {"четверг",     4},
                {"пятница",     5},
                {"суббота",     6},
                {"воскресенье", 0},
            };

            if (DayOfWeekDict.TryGetValue(
                ruDayOfWeekName.ToLower().Replace(" ", ""), out var inputDayOfWeek))
            {

                int dateDayOfWeek = (int)date.DayOfWeek;

                if (dateDayOfWeek == 0 & inputDayOfWeek == 0)
                    return date;
                if (dateDayOfWeek == 0)
                    return date.AddDays(-(7 - inputDayOfWeek));
                if (inputDayOfWeek == 0)
                    return date.AddDays(7 - dateDayOfWeek);
                else
                    return date.AddDays(inputDayOfWeek - dateDayOfWeek);
            }
            else
                return default;


        }

        public static bool TryGetDate(string dateString, out DateOnly date)
        {
            if (string.IsNullOrEmpty(dateString))
                return false;

            dateString = dateString.ToLower().TrimStart();

            var monthName = _monthRE.Match(dateString).Value.Replace(" ", "");

            if (monthName.EndsWith('а'))
                monthName = monthName.Remove(monthName.Length - 1);
            else
                monthName = monthName.Remove(monthName.Length - 1, 1) + "ь";

            string replacementString = string.Empty;

            if (_monthNamesDict.TryGetValue(monthName, out var m))
                replacementString = $"/{m}/";
            else
                return false;

            dateString = dateString.Replace(_monthRE.Match(dateString).Value, replacementString);

            return DateOnly.TryParse(dateString, out date);
        }
        public static DateOnly GetDate(string date)
        {

            date = date.ToLower().TrimStart();

            var monthName = _monthRE.Match(date).Value.Replace(" ", "");

            if (monthName.EndsWith('а'))
                monthName = monthName.Remove(monthName.Length - 1);
            else
                monthName = monthName.Remove(monthName.Length - 1, 1) + "ь";

            string replacementString = string.Empty;

            if (_monthNamesDict.TryGetValue(monthName, out var m))
                replacementString = $"/{m}/";
            else
                throw new Exception("Could not parse date");

            date = date.Replace(_monthRE.Match(date).Value, replacementString);

            DateOnly newDate = new();
            if (DateOnly.TryParse(date, out var realDate))
                return realDate;
            else
                return newDate;
        }


        public static DateOnly GetMondayByWeekNumber(int weekNumber)
        {
            var firstOfSeptember = new DateOnly(DateTime.Now.Year, 9, 1);
            var firstWeekMonday = firstOfSeptember.GetDateByDayOfWeek("понедельник");

            var outDate = firstWeekMonday.AddDays((weekNumber - 1) * 7);

            return outDate;
        }


    }
}
