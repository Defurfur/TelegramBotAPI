﻿using ReaSchedule.Models;
using ScheduleUpdateService.Extensions;
using System.Text;
using System.Text.RegularExpressions;
using TelegramBotService.Abstractions;
using XAct;

namespace TelegramBotService.Services;

public class ScheduleFormatter : IScheduleFormatter
{
    private StringBuilder? _sb;
    private StringBuilder Sb { get => _sb ??= new(1000, 4096); }

    public string Format(ReaGroup reaGroup)
    {
        FormatReaGroup(reaGroup);
        var result = Sb.ToString();
        result = EscapeCharacters(result);
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

        for(var i = 0; i < orderedList.Count; i++)
        {
            var reaClass = orderedList[i];

            if(i != 0 
                && reaClass.OrdinalNumber == orderedList[i - 1].OrdinalNumber)
            {
                ProcessDoubledClass(reaClass);
                return;
            }

            ProcessSingleClass(reaClass);
           
            //TODO: test these 2 methods
        }

        void ProcessDoubledClass(ReaClass reaClass)
        {
            if (reaClass.Subgroup is not null)
                Sb.Append("Подруппа: " + reaClass.Subgroup + "\r\n");

            Sb.Append("Аудитория: " + reaClass.Audition + "\r\n");

            Sb.Append
            (
                CapitalizeEachWordFirstLetter(reaClass.Professor) + "\r\n\r\n"
            );
        }


        void ProcessSingleClass(ReaClass reaClass)
        {
            Sb.Append(reaClass.OrdinalNumber + "\r\n\r\n");
            Sb.Append(reaClass.ClassName + "\r\n");
            Sb.Append(reaClass.ClassType + "\r\n");

            if (reaClass.Subgroup is not null)
                Sb.Append("Подруппа: " + reaClass.Subgroup + "\r\n");

            Sb.Append("Аудитория: " + reaClass.Audition + "\r\n");
            Sb.Append
            (
                CapitalizeEachWordFirstLetter(reaClass.Professor) + "\r\n\r\n"
            );
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
        Sb.Append($"*Расписание для группы {reaGroup.GroupName.ToUpper()}*\r\n");

        foreach (var scheduleWeek in reaGroup.ScheduleWeeks!)
        {
            FormatScheduleWeek(scheduleWeek);
        }
    }

    private void FormatScheduleDay(ScheduleDay scheduleDay)
    {
        Sb.Append($"\r\n*{CapitalizeFirstLetter(scheduleDay.DayOfWeekName)}*\r\n");

        if (scheduleDay.IsEmpty)
            Sb.Append("Занятия отсутствуют\r\n");
        else
            FormatReaClasses(scheduleDay.ReaClasses);

    }

    private void FormatScheduleWeek(ScheduleWeek scheduleWeek)
    {
        var weekNumberAsString = scheduleWeek.WeekStart.GetWeekNumber().ToString();

        Sb.Append("*Расписание на " + weekNumberAsString + " Неделю*\r\n" 
            + $"{scheduleWeek.WeekStart} - {scheduleWeek.WeekEnd}\r\n");

        foreach (var scheduleDay in scheduleWeek.GetScheduleDays())
        {
            FormatScheduleDay(scheduleDay);
        }
    }

    private string EscapeCharacters(string text)
    {
        return text
            .Replace(".", """\.""")
            .Replace("-", """\-""");
    }

    private string CapitalizeFirstLetter(string text)
    {
        return Regex.Replace(text, "^[а-я]", m => m.Value.ToUpper());
    }

    private string CapitalizeEachWordFirstLetter(string text)
    {
        string[] words = text.Split(" ");
        StringBuilder sb = new();

        foreach (var word in words)
        {
            var newWord = Regex.Replace(word, "^[а-я]", m => m.Value.ToUpper());
            sb.Append(newWord + " ");
            
        }

        return sb.ToString();
    }
}
