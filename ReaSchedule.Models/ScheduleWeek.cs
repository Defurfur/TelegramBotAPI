using System;

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
    public int Id { get; set; }
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public ScheduleDay Monday { get; set; }
    public ScheduleDay Tuesday { get; set; }
    public ScheduleDay Wednesday { get; set; }
    public ScheduleDay Thursday { get; set; }
    public ScheduleDay Friday { get; set; }
    public ScheduleDay Saturday { get; set; }
   


}


public class ScheduleDay
{

    public string DayOfWeekName { get; set; } = string.Empty;
    public ICollection<ReaClass> ReaClasses { get; set; } = new List<ReaClass>();
    public DateOnly Date { get; set; }
    public bool IsEmpty => ReaClasses == null || ReaClasses.Count == 0;

}
