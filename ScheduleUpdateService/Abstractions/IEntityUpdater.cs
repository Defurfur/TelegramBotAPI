using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions
{
    public interface IEntityUpdater
    {
        ReaGroup UpdateReaGroup(
            ReaGroup reaGroup,
            List<WeeklyClassesWrapper> weeklyClasses);

    }
}
