using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace ReaSchedule.Models;
public class ScheduleWeek : IIdentifyable<int>
{
    public ScheduleWeek() 
    {
        Monday.DayOfWeekName = "понедельник";
        Tuesday.DayOfWeekName = "вторник";
        Wednesday.DayOfWeekName = "среда";
        Thursday.DayOfWeekName = "четверг";
        Friday.DayOfWeekName = "пятница";
        Saturday.DayOfWeekName = "суббота";
    }
    #region DayOfWeek Backing fields
    [NotMapped]
    private ScheduleDay _monday;
    [NotMapped]
    private ScheduleDay _tuesday;
    [NotMapped]
    private ScheduleDay _wednesday;
    [NotMapped]
    private ScheduleDay _thursday;
    [NotMapped]
    private ScheduleDay _friday;
    [NotMapped]
    private ScheduleDay _saturday;
    #endregion
    public int Id { get; set; }
    public int ReaGroupId { get; set; }
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public List<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>
    { 
        new ScheduleDay(){DayOfWeek = DayOfWeek.Monday },    
        new ScheduleDay(){DayOfWeek = DayOfWeek.Tuesday },    
        new ScheduleDay(){DayOfWeek = DayOfWeek.Wednesday },    
        new ScheduleDay(){DayOfWeek = DayOfWeek.Thursday },    
        new ScheduleDay(){DayOfWeek = DayOfWeek.Friday},    
        new ScheduleDay(){DayOfWeek = DayOfWeek.Saturday},
    
    };
    #region DayOfWeek Properties
    [NotMapped]
    public ScheduleDay Monday 
    { 
        get => _monday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Monday); 
        set { _monday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Monday); _monday = value; } 
    }
    [NotMapped]
    public ScheduleDay Tuesday
    {
        get => _tuesday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Tuesday);
        set { _tuesday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Tuesday); _tuesday = value; }
    }
    [NotMapped]
    public ScheduleDay Wednesday
    {
        get => _wednesday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Wednesday);
        set { _wednesday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Wednesday); _wednesday = value; }
    }
    [NotMapped]
    public ScheduleDay Thursday
    {
        get => _thursday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Thursday);
        set { _thursday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Thursday); _thursday = value; }
    }
    [NotMapped]
    public ScheduleDay Friday
    {
        get => _friday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Friday);
        set { _friday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Friday); _friday = value; }
    }
    [NotMapped]
    public ScheduleDay Saturday
    {
        get => _saturday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Saturday);
        set { _saturday ??= ScheduleDays.First(x => x.DayOfWeek == DayOfWeek.Saturday); _saturday = value; }
    }
    #endregion
    public IEnumerable<ScheduleDay> GetScheduleDays() 
    {
        return ScheduleDays;
    }

}

public class ScheduleDay : IIdentifyable<int>
{

    public int Id { get; set; }
    public int ScheduleWeekId { get; set; }
    public ScheduleWeek? ScheduleWeek { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public ICollection<ReaClass> ReaClasses { get; set; } = new List<ReaClass>();
    public DateOnly Date { get; set; }
    public bool IsEmpty => !ReaClasses.Any();

}
