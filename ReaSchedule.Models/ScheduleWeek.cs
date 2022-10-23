using System;

namespace ReaSchedule.Models;
public class ScheduleWeek : IIdentifyable<int>
{
    public int Id { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public ScheduleDay? Monday { get; set; }
    public ScheduleDay? Tuesday { get; set; }
    public ScheduleDay? Wednesday { get; set; }
    public ScheduleDay? Thursday { get; set; }
    public ScheduleDay? Friday { get; set; }
    public ScheduleDay? Saturday { get; set; }
   


}


public class ScheduleDay
{
    public ICollection<ReaClass>? ReaClasses { get; set; }
    public DateTime Date { get; set; }
    public bool isEmpty => ReaClasses == null || ReaClasses.Count == 0;

}
