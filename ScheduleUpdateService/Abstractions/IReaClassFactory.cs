using ReaSchedule.Models;

namespace ScheduleUpdateService.Abstractions
{
    public interface IReaClassFactory
    {
        public ReaClass CreateInstance(string classInfo);
        public List<ReaClass> CreateFromList(IEnumerable<string> array);
    }
}
