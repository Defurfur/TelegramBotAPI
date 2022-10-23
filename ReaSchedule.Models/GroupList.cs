using System;

namespace ReaSchedule.Models;
public class GroupList : IIdentifyable<int>
{
    public int Id { get; set; }
    public string GroupName { get; set; } = string.Empty;

}
