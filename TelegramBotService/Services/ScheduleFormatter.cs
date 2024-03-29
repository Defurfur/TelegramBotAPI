﻿using ReaSchedule.Models;
using ScheduleUpdateService.Extensions;
using System.Text;
using System.Text.RegularExpressions;
using TelegramBotService.Abstractions;
using XAct;
using TelegramEmoji;
using Humanizer;
using System.Globalization;

namespace TelegramBotService.Services;

public class ScheduleFormatter : IScheduleFormatter
{
    private readonly CultureInfo _ruCulture = new CultureInfo("ru-RU");
    private StringBuilder? _sb;
    private StringBuilder Sb { get => _sb ??= new(1000, 4096); }

    public string Format(ReaGroup reaGroup)
    {
        FormatReaGroup(reaGroup);
        var result = EscapeCharacters(Sb.ToString());
        Sb.Clear();
        return result;
    }
    public string Format(List<ReaClass> reaClasses)
    {
        FormatReaClasses(reaClasses);
        var result = EscapeCharacters(Sb.ToString());
        Sb.Clear();
        return result;
    }
    public string Format(ScheduleWeek scheduleWeek)
    {
        FormatScheduleWeek(scheduleWeek);
        var result = EscapeCharacters(Sb.ToString());
        Sb.Clear();
        return result;
    }
    public string Format(ScheduleDay scheduleDay)
    {
        FormatScheduleDay(scheduleDay);
        var result = EscapeCharacters(Sb.ToString());
        Sb.Clear();
        return result;
    }  
    public string Format(List<ScheduleDay> scheduleDays)
    {
        foreach(var scheduleDay in scheduleDays)
            FormatScheduleDay(scheduleDay);
        var result = EscapeCharacters(Sb.ToString());
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
                continue;
            }
            
            ProcessSingleClass(reaClass);

        }

        void ProcessDoubledClass(ReaClass reaClass)
        {
            if (!reaClass.Subgroup.IsNullOrEmpty())
                Sb.Append("\r\nПодруппа: " + reaClass.Subgroup + "\r\n");

            Sb.Append("Аудитория: " + reaClass.Audition.Replace("  ","") + "\r\n");

            Sb.Append(reaClass.Professor.Humanize(LetterCasing.Title) + "\r\n");
        }


        void ProcessSingleClass(ReaClass reaClass)
        {
            Sb.Append("\r\n" +
                DictionaryStorage
                .OrdinalNumberAndEmojiDict
                .GetValue(reaClass.OrdinalNumber, false) + "\r\n");

            Sb.Append($"*{reaClass.ClassName}*\r\n");
            Sb.Append($"_{reaClass.ClassType}_\r\n");

            if (!reaClass.Subgroup.IsNullOrEmpty())
                Sb.Append("Подруппа: " + reaClass.Subgroup + "\r\n");

            Sb.Append("Аудитория: " + reaClass.Audition.Replace("  ","") + "\r\n");

            Sb.Append(reaClass.Professor.Humanize(LetterCasing.Title) + "\r\n");
        }

    }

    private List<ReaClass> OrderReaClassByOrdinalNumber(List<ReaClass> reaClasses)
    {
        var newItems = new List<ReaClass>();
        var newList = new List<ReaClass>();

        for(var i = 1; i <= 8; i++)
        {
            newItems = reaClasses
                .Where(x => x.OrdinalNumber.Contains($"{i}"))
                .ToList();

            if (newItems != null && newItems.Any())
                newList.AddRange(newItems);
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

        var DayOfWeek = scheduleDay.DayOfWeekName.ToUpper();

        string str =
            scheduleDay.Date == DateOnly.FromDateTime(DateTime.Now)
            ? $"\r\n\u2757 -*{DayOfWeek} {scheduleDay.Date.ToString(_ruCulture)}*-\r\n"
            : $"\r\n-*{DayOfWeek} {scheduleDay.Date.ToString(_ruCulture)}*-\r\n";

        Sb.Append(str);

        if (scheduleDay.IsEmpty)
            Sb.Append("Занятия отсутствуют\r\n");
        else
            FormatReaClasses(scheduleDay.ReaClasses);

    }

    private void FormatScheduleWeek(ScheduleWeek scheduleWeek)
    {
        var weekNumberAsString = scheduleWeek.WeekStart.GetWeekNumber().ToString();

        Sb.Append("\r\n*Расписание на " + weekNumberAsString + " Неделю*\r\n" 
            + $"{scheduleWeek.WeekStart.ToString(_ruCulture)} - {scheduleWeek.WeekEnd.ToString(_ruCulture)}\r\n");

        foreach (var scheduleDay in scheduleWeek.GetScheduleDays())
        {
            FormatScheduleDay(scheduleDay);
        }
    }

    private string EscapeCharacters(string text)
    {
        return text
            .Replace("""\""", """\\""")
            .Replace(".", """\.""")
            .Replace("-", """\-""")
            .Replace("+", """\+""")
            .Replace("(", """\(""")
            .Replace(")", """\)""");
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
