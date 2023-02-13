using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Extensions;
using System.Text.RegularExpressions;

namespace ScheduleUpdateService.Services
{
    public class ScheduleWeekFactory : IScheduleWeekFactory
    {
        private readonly IReaClassFactory _reaClassFactory;
        private readonly Regex _dayOfWeekRE = new(@"(понедельник|вторник|среда|четверг|пятница|суббота)");
        private readonly Regex _dateRE = 
            new(@"\d{1,2}\s*(марта|апреля|мая|июня|июля|августа|сентября|октября|ноября|декабря|января|февраля)\s*\d{4}");

        public ScheduleWeekFactory(IReaClassFactory reaClassFactory)
        {
            _reaClassFactory = reaClassFactory;
        }
        private void FillScheduleDay(ScheduleDay scheduleDay, List<string> processList, int weekNumber)
        {

            var sortedClassInfoArray = processList!
                .Where(x => _dayOfWeekRE.Match(x).Value == scheduleDay.DayOfWeekName)
                .ToList();

            if (sortedClassInfoArray is null || !sortedClassInfoArray.Any())
            {
                var now = DateTime.Now;

                int year = 9 <= now.Month && now.Month <= 12 
                    ? now.Year 
                    : now.Year - 1;

                scheduleDay.Date = DateTimeExtension
                    .GetMondayByWeekNumber(weekNumber, year)
                    .GetDateByDayOfWeek(scheduleDay.DayOfWeekName);
            }
            else
            {
                var firstClass = sortedClassInfoArray!.First();
                var firstClassDateAsString = _dateRE.Match(firstClass).Value;

                scheduleDay.Date = DateTimeExtension.GetDate(firstClassDateAsString);
                scheduleDay.ReaClasses = _reaClassFactory.CreateFromList(sortedClassInfoArray!);

                processList.RemoveAll(x => sortedClassInfoArray!.Contains(x));
            }


        }
        public ScheduleWeek CreateInstance(WeeklyClassesWrapper weeklyClassesWrapper)
        {

            var scheduleWeek = new ScheduleWeek(createDefaultScheduleDays: true);

            var processingClassInfoArray = weeklyClassesWrapper.WeeklyClasses;

            if (processingClassInfoArray is null)
                return scheduleWeek;

            foreach (ScheduleDay scheduleDay in scheduleWeek.GetScheduleDays())
                FillScheduleDay(scheduleDay, processingClassInfoArray, weeklyClassesWrapper.WeekNumber);

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
