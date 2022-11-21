using ReaSchedule.Models;
using System.Text.RegularExpressions;

namespace ScheduleUpdateService.Services
{

    public interface IScheduleWeekFactory
    {
        public List<ScheduleWeek> CreateMany(List<WeeklyClassesWrapper> weeklyClassesWrappers);
        public ScheduleWeek CreateInstance(WeeklyClassesWrapper weeklyClassesWrapper);

    }
    public class ScheduleWeekFactory : IScheduleWeekFactory
    {
        private readonly IReaClassFactory _reaClassFactory;
        private List<string> _processingClassInfoArray = new();
        private readonly Regex _dayOfWeekRE = new(@"(?<=(</strong>[\Wa-z]+))([а-я]+)");
        private readonly Regex _dateRE = new(@"(?<=,)([0-9 а-я]+)(?=,)");

        public ScheduleWeekFactory(IReaClassFactory reaClassFactory)
        {
            _reaClassFactory = reaClassFactory;
        }
        private void FillScheduleDay(ScheduleDay scheduleDay, int weekNumber)
        {

            var sortedClassInfoArray = _processingClassInfoArray!
                .Where(x => _dayOfWeekRE.Match(x).Value == scheduleDay.DayOfWeekName)
                .ToList();

            if (sortedClassInfoArray is null || !sortedClassInfoArray.Any())
            {
                scheduleDay.Date = DateTimeExtension
                    .GetMondayByWeekNumber(weekNumber)
                    .GetDateByDayOfWeek(scheduleDay.DayOfWeekName);
            }
            else
            {
                var firstClass = sortedClassInfoArray!.First();
                var firstClassDateAsString = _dateRE.Match(firstClass).Value;

                scheduleDay.Date = DateTimeExtension.GetDate(firstClassDateAsString);
                scheduleDay.ReaClasses = _reaClassFactory.CreateFromList(sortedClassInfoArray!);

                _processingClassInfoArray.RemoveAll(x => sortedClassInfoArray!.Contains(x));
            }


        }
        public ScheduleWeek CreateInstance(WeeklyClassesWrapper weeklyClassesWrapper)
        {

            var scheduleWeek = new ScheduleWeek();
            _processingClassInfoArray = weeklyClassesWrapper.WeeklyClasses;

            if (_processingClassInfoArray is null)
                return scheduleWeek;

            foreach (ScheduleDay scheduleDay in scheduleWeek.GetScheduleDays())
                FillScheduleDay(scheduleDay, weeklyClassesWrapper.WeekNumber);

            scheduleWeek.WeekStart = scheduleWeek.Monday.Date;
            scheduleWeek.WeekEnd = scheduleWeek.Saturday.Date.AddDays(1);

            return scheduleWeek;

        }

        public List<ScheduleWeek> CreateMany(List<WeeklyClassesWrapper> weeklyClassesList)
        {
            var scheduleWeeks = new List<ScheduleWeek>();

            foreach (var weeklyClassesWrapper in weeklyClassesList)
            {
                var scheduleWeek = CreateInstance(weeklyClassesWrapper);
                scheduleWeeks.Add(scheduleWeek);
            }

            return scheduleWeeks;
        }
    }

}
