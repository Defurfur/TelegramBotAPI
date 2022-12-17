namespace ReaSchedule.Models;

public class SubscriptionSettings: IIdentifyable<int>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool SubscriptionEnabled { get; set; }
    public UpdateSchedule UpdateSchedule { get; set; } = UpdateSchedule.NotSet;
    public DayOfWeekEx? DayOfUpdate { get; set; } = null;
    public DayAmountToUpdate DayAmountToUpdate { get; set; } = DayAmountToUpdate.NotSet;
    public TimeOfDay TimeOfDay { get; set; } = TimeOfDay.NotSet;
    public WeekToSend WeekToSend { get; set; } = WeekToSend.NotSet;
    public bool IncludeToday { get; set; }
    public bool DailySettingsValid
    {
        get =>
        UpdateSchedule == UpdateSchedule.EveryDay
        && DayOfUpdate == null
        && DayAmountToUpdate != DayAmountToUpdate.NotSet
        && TimeOfDay != TimeOfDay.NotSet    
        && WeekToSend == WeekToSend.NotSet;
    }
    public bool WeeklySettingsValid
    {
        get =>
        UpdateSchedule == UpdateSchedule.EveryWeek
        && DayOfUpdate != null
        && DayAmountToUpdate == DayAmountToUpdate.NotSet
        && TimeOfDay != TimeOfDay.NotSet    
        && WeekToSend != WeekToSend.NotSet;
    }



    public void Reset()
    {
        SubscriptionEnabled = false;
        UpdateSchedule= UpdateSchedule.NotSet;
        DayOfUpdate = null;
        WeekToSend= WeekToSend.NotSet;
        DayAmountToUpdate = DayAmountToUpdate.NotSet;
        TimeOfDay = TimeOfDay.NotSet;
        IncludeToday = false;
    }
    public void Reset(bool enableSubscription)
    {
        Reset();
        SubscriptionEnabled = enableSubscription;

    }

    public void DisableSubscription() => SubscriptionEnabled = false;
    public void EnableSubscription() => SubscriptionEnabled = true;
    public void Update(UpdateSchedule updateSchedule) => UpdateSchedule= updateSchedule;
    public void Update(DayOfWeekEx dayOfUpdate) => DayOfUpdate = dayOfUpdate;
    public void Update(DayAmountToUpdate dayAmountToUpdate) => DayAmountToUpdate = dayAmountToUpdate;
    public void Update(TimeOfDay timeOfDay) => TimeOfDay = timeOfDay;
    public void Update(WeekToSend weekToSend) => WeekToSend = weekToSend;
    public void Update(bool includeToday) => IncludeToday = includeToday;

}