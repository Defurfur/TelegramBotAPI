using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions
{
    public interface IScheduleWeekFactory
    {
        public List<ScheduleWeek> CreateMany(List<WeeklyClassesWrapper> weeklyClassesWrappers);
        public ScheduleWeek CreateInstance(WeeklyClassesWrapper weeklyClassesWrapper);

    }

}
