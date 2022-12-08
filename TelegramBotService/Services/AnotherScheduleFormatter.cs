using ReaSchedule.Models;
using ScheduleUpdateService.Extensions;
using System.Text;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services;

public class AnotherScheduleFormatter : IScheduleFormatter
{
    private StringBuilder? _sb;
    private StringBuilder Sb { get => _sb ??= new(1000, 4096); }

    public string Format(ReaGroup reaGroup)
    {
        FormatReaGroup(reaGroup);
        var result = Sb.ToString();
        Sb.Clear();
        return result;
    }
    public string Format(List<ReaClass> reaClasses)
    {
        FormatReaClasses(reaClasses);
        var result = Sb.ToString();
        Sb.Clear();
        return result;
    }
    public string Format(ScheduleWeek scheduleWeek)
    {
        FormatScheduleWeek(scheduleWeek);
        var result = Sb.ToString();
        Sb.Clear();
        return result;
    }
    public string Format(ScheduleDay scheduleDay)
    {
        FormatScheduleDay(scheduleDay);
        var result = Sb.ToString();
        Sb.Clear();
        return result;
    }

    private void FormatReaClasses(List<ReaClass> reaClasses)
    {

        var orderedList = OrderReaClassByOrdinalNumber(reaClasses);

        foreach (var reaClass in orderedList)
        {
            Sb.Append(reaClass.OrdinalNumber + "\r\n\r\n");
            Sb.Append(reaClass.ClassName + "\r\n");
            Sb.Append(reaClass.ClassType + "\r\n");
            Sb.Append("Аудитория: " + reaClass.Audition + "\r\n");
            Sb.Append(reaClass.Professor + "\r\n");
            if (reaClass.Subgroup is not null)
                Sb.Append(reaClass.Subgroup + "\r\n");

        }
    }

    private List<ReaClass> OrderReaClassByOrdinalNumber(List<ReaClass> reaClasses)
    {
        var newList = new List<ReaClass>();
        for(var i = 1; i <= 8; i++)
        {
            var newItem = reaClasses.FirstOrDefault(x => x.OrdinalNumber.Contains($"{i}"));
            if (newItem != null)
                newList.Add(newItem);
        }
        return newList;
    } 
    private void FormatReaGroup(ReaGroup reaGroup)
    {
        Sb.Append($"Расписание для группы {reaGroup.GroupName.ToUpper()}");

        foreach (var scheduleWeek in reaGroup.ScheduleWeeks!)
        {
            FormatScheduleWeek(scheduleWeek);
        }
    }

    private void FormatScheduleDay(ScheduleDay scheduleDay)
    {
        Sb.Append(scheduleDay.DayOfWeekName + "\r\n");
        if (scheduleDay.IsEmpty)
            Sb.Append("Занятия отсутствуют");
        else
            FormatReaClasses(scheduleDay.ReaClasses);

    }

    private void FormatScheduleWeek(ScheduleWeek scheduleWeek)
    {
        var weekNumberAsString = scheduleWeek.WeekStart.GetWeekNumber().ToString();

        Sb.Append("Неделя " + weekNumberAsString + ":" + $"{scheduleWeek.WeekStart} - {scheduleWeek.WeekEnd}");

        foreach (var scheduleDay in scheduleWeek.GetScheduleDays())
        {
            FormatScheduleDay(scheduleDay);
        }
    }
}
