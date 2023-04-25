using System.Text.RegularExpressions;
using XAct;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleUpdateService.Extensions;

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
    private static readonly Dictionary<string, int> _dayOfWeekDict = new()
    {
            {"понедельник", 1},
            {"вторник",     2},
            {"среда",       3},
            {"четверг",     4},
            {"пятница",     5},
            {"суббота",     6},
            {"воскресенье", 0},
    };
    public static int GetWeekNumber(this DateTime dateTime)
    {
        DateOnly today = DateOnly.FromDateTime(dateTime);
        return today.GetWeekNumber();

    }
    /// <summary>
    /// Gets the number of the studying week in the university. Used on the schedule website.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int GetWeekNumber(this DateOnly date)
    {
        // returns first of september - date of the start of the studying year
        
        DateOnly firstOfSeptember = 
            date.Month >= 9 && date.Month <= 12
            ? new(date.Year, 9, 1)
            : new(date.Year - 1, 9, 1);

        var currentWeekStart = date.GetWeekStart();

        var firstWeekStart = firstOfSeptember.GetWeekStart();

        var weekNumber = (currentWeekStart.DayNumber - firstOfSeptember.DayNumber) / 7 + 2;

        return weekNumber;

    }
    public static DateOnly GetWeekStart(this DateOnly date)
    {
        int dateDayOfWeek = (int)date.DayOfWeek;

        int dayDifference = 
            dateDayOfWeek == 0 
            ? -6 
            : 1 - dateDayOfWeek;

        var weekStartDate = date.AddDays(dayDifference);

        return weekStartDate;

    }

    public static DateOnly GetWeekEnd(this DateOnly date)
    {
        int dateDayOfWeek = (int)date.DayOfWeek;

        int dayDifference = 
            dateDayOfWeek == 0 
            ? 0 
            : 7 - dateDayOfWeek;

        var weekEndDate = date.AddDays(dayDifference);

        return weekEndDate;

    }

    public static DateOnly GetDateByDayOfWeek(this DateOnly date, string ruDayOfWeekName)
    {
        int inputDayOfWeek;

        ruDayOfWeekName = ruDayOfWeekName.ToLower().Trim();

        inputDayOfWeek = ruDayOfWeekName switch
        {
            "понедельник" => 1,
            "вторник" => 2,
            "среда" => 3,
            "четверг" => 4,
            "пятница" => 5,
            "суббота" => 6,
            "воскресенье" => 0,
            _ => throw new ArgumentException("Invalid input", nameof(ruDayOfWeekName))
        };

        int dateDayOfWeek = (int)date.DayOfWeek;

        //Case when both input and target dates are the same sunday
        if(dateDayOfWeek == 0 & inputDayOfWeek == 0)
        {
            return date;
        };

        //Case when input date day of week is sunday
        if(dateDayOfWeek == 0)
        {
            return date.AddDays(-(7 - inputDayOfWeek));
        };

        //Case when method argument is "воскресенье"
        if (inputDayOfWeek == 0)
        {
            return date.AddDays(7 - dateDayOfWeek);
        };

        //Other cases
        return date.AddDays(inputDayOfWeek - dateDayOfWeek);

    }


    public static DateOnly GetDate(string date)
    {

        date = date.ToLower().Trim();

        var splittedDate = date.Split(' ');

        if (splittedDate.Length != 3)
            return default;

        var day = int.Parse(splittedDate[0]);
        var year = int.Parse(splittedDate[2]);
        var monthAsString = splittedDate[1];

        if (monthAsString.EndsWith('а'))
            monthAsString = monthAsString.Remove(monthAsString.Length - 1);
        else if (monthAsString == "мая")
            monthAsString = "май";
        else
            monthAsString = monthAsString.Remove(monthAsString.Length - 1, 1) + "ь";

        _ = _monthNamesDict.TryGetValue(monthAsString, out var month);

        if(month == 0 || day == 0 || year == 0)
            return default;

        return new DateOnly(year, month, day);

    }


    public static DateOnly GetMondayByWeekNumber(int weekNumber, int startYear)
    {

        DateOnly firstOfSeptember = new(startYear, 9, 1);

        var firstWeekMonday = firstOfSeptember.GetDateByDayOfWeek("понедельник");

        var outDate = firstWeekMonday.AddDays((weekNumber - 1) * 7);

        return outDate;
    }


}
