using System;

namespace ReaSchedule.Models;
public class ScheduledTask : IIdentifyable<int>
{
    public ScheduledTask() { }
    public ScheduledTask(
        string taskName,
        DateTime dateTime,
        string message = "",
        int entryNumber = 0,
        int actionNumber = 0,
        string exception = "",
        int elapsed = 0)
    {
        DateTime = dateTime;
        TaskName = taskName;
        Message = message;
        EntryNumber = entryNumber;
        ActionNumber = actionNumber;
        Exception = exception;
        Elapsed = elapsed;
    }

    public int Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Message { get; set; }  = string.Empty;
    public DateTime DateTime { get; set; }
    public int EntryNumber { get; set; }
    public int ActionNumber { get; set; }
    public string Exception { get; set; } = string.Empty;
    public int Elapsed { get; set; }
}
