using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;

namespace ScheduleUpdateService.Services
{
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
