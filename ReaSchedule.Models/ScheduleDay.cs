using System;

namespace ReaSchedule.Models;
public class booba : IIdentifyable<int>
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DayOfTheWeek DayOfTheWeek { get; set; }
    public bool IsFree { get; set; }
    public ICollection<ReaClass>? ReaClasses { get; set; }

}

public enum DayOfTheWeek
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}