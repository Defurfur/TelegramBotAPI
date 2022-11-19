using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaSchedule.Models;

public class User : IIdentifyable<int>
{
    public int Id { get; set; }
    public int ReaGroupId { get; set; }
    public ReaGroup? ReaGroup { get; set; }
    public long ChatId { get; set; }
    public bool SubscriptionEnabled { get; set; }
    public UpdateSchedule? UpdateSchedule { get; set;}
    public DayOfWeek? DayOfUpdate { get; set; }
    public DayNumberToUpdate? DayNumberToUpdate { get; set; }

}
public enum UpdateSchedule
{
    EveryDay,
    EveryWeek, 
}
public enum DayNumberToUpdate
{
    OneDay,
    TwoDays,
    ThreeDays,
}