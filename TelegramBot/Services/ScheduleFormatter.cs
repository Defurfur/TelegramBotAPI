using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReaSchedule.Models;
using ScheduleWorker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Abstractions;

namespace TelegramBot.Services;

public class ScheduleFormatter : IScheduleFormatter
{
    private StringBuilder? _sb;
    private StringBuilder sb { get => _sb ??= new(1000, 10_000); }

    public string Format(ReaGroup reaGroup)
    {
        FormatReaGroup(reaGroup);
        var result = sb.ToString();
        sb.Clear();
        return result;
    }
    public string Format(List<ReaClass> reaClasses)
    {
        FormatReaClasses(reaClasses);
        var result = sb.ToString();
        sb.Clear();
        return result;
    }
    public string Format(ScheduleWeek scheduleWeek)
    {
        FormatScheduleWeek(scheduleWeek);
        var result = sb.ToString();
        sb.Clear();
        return result;
    }
    public string Format(ScheduleDay scheduleDay)
    {
        FormatScheduleDay(scheduleDay);
        var result = sb.ToString();
        sb.Clear();
        return result;
    }

    private void FormatReaClasses(List<ReaClass> reaClasses)
    {
        reaClasses.OrderBy(x => x.OrdinalNumber);

        foreach (var reaClass in reaClasses)
        {
            sb.Append(reaClass.OrdinalNumber + "\r\n\r\n");
            sb.Append(reaClass.ClassName + "\r\n");
            sb.Append(reaClass.ClassType + "\r\n");
            sb.Append("Аудитория: " + reaClass.Audition + "\r\n");
            sb.Append(reaClass.Professor + "\r\n");
            if (reaClass.Subgroup is not null)
                sb.Append(reaClass.Subgroup + "\r\n");

        }
    }
    private void FormatReaGroup(ReaGroup reaGroup)
    {
        sb.Append($"Расписание для группы {reaGroup.GroupName.ToUpper()}");

        foreach (var scheduleWeek in reaGroup.ScheduleWeeks!)
        {
            FormatScheduleWeek(scheduleWeek);
        }
    }

    private void FormatScheduleDay(ScheduleDay scheduleDay)
    {
        sb.Append(scheduleDay.DayOfWeekName + "\r\n");
        if (scheduleDay.IsEmpty)
            sb.Append("Занятия отсутствуют");
        else
            FormatReaClasses(scheduleDay.ReaClasses);

    }

    private void FormatScheduleWeek(ScheduleWeek scheduleWeek)
    {
        var weekNumberAsString = scheduleWeek.WeekStart.GetWeekNumber().ToString();

        sb.Append("Неделя " + weekNumberAsString + ":" + $"{scheduleWeek.WeekStart} - {scheduleWeek.WeekEnd}");

        foreach (var scheduleDay in scheduleWeek.GetScheduleDays())
        {
            FormatScheduleDay(scheduleDay);
        }
    }
}
