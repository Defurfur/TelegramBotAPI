using System;

namespace ReaSchedule.Models;
public class ReaGroup : IIdentifyable<int>
{
    public int Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public ICollection<ScheduleWeek>? ScheduleWeeks { get; set; }
    public string Hash { get; set; } = string.Empty;


}
