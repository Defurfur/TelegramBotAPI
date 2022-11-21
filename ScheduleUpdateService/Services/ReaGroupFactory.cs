using ReaSchedule.Models;

namespace ScheduleUpdateService.Services
{

    public interface IReaGroupFactory
    {
        ReaGroup CreateReaGroup(List<ScheduleWeek> scheduleWeeks);
    }
    public class ReaGroupFactory : IReaGroupFactory
    {
        private readonly IHashingService _hashingService;

        public ReaGroupFactory(IHashingService hashingService)
        {
            _hashingService = hashingService;
        }

        public ReaGroup CreateReaGroup(List<ScheduleWeek> scheduleWeeks)
        {
            ReaGroup newReaGroup = new();
            newReaGroup.ScheduleWeeks = scheduleWeeks;
            newReaGroup.Hash = _hashingService.GetHashSum(newReaGroup);
            return newReaGroup;
        }
    }
}
