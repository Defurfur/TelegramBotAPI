using ReaSchedule.Models;

namespace ScheduleUpdateService.Services
{
    public interface IEntityUpdater
    {
        ReaGroup UpdateReaGroup(
            ReaGroup reaGroup,
            List<WeeklyClassesWrapper> weeklyClasses);

    }

    public class EntityUpdater : IEntityUpdater
    {
        private readonly IScheduleWeekFactory _scheduleWeekFactory;
        private readonly IReaGroupFactory _reaGroupFactory;
        public EntityUpdater(
            IScheduleWeekFactory scheduleWeekFactory,
            IReaGroupFactory reaGroupFactory)
        {
            _scheduleWeekFactory = scheduleWeekFactory;
            _reaGroupFactory = reaGroupFactory;
        }


        public ReaGroup UpdateReaGroup(
            ReaGroup reaGroup,
            List<WeeklyClassesWrapper> weeklyClasses)
        {
            if (weeklyClasses == null || !weeklyClasses.Any())
                throw new ArgumentException("Cannot update ReaGroup with no data", nameof(weeklyClasses));
            if (reaGroup.GroupName == string.Empty)
                throw new ArgumentException("Input ReaGroup has no GroupName", nameof(reaGroup.GroupName));

            var scheduleWeeks = _scheduleWeekFactory.CreateMany(weeklyClasses);
            var updatedReaGroup = _reaGroupFactory.CreateReaGroup(scheduleWeeks);

            updatedReaGroup.GroupName = reaGroup.GroupName;
            updatedReaGroup.Id = reaGroup.Id;

            return updatedReaGroup;

        }

    }
}
