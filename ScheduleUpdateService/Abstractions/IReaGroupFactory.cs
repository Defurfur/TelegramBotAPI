using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions
{
    public interface IReaGroupFactory
    {
        ReaGroup CreateReaGroup(List<ScheduleWeek> scheduleWeeks);
    }
}
