using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleWorker.Services
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
