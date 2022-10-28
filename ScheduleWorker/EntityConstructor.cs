using ReaSchedule.Models;
using System.Text.RegularExpressions;

namespace ScheduleWorker
{
    public interface IEntityConstructor
    {
        List<ScheduleWeek> ConstructScheduleWeeks(List<string> classInfoArray);
        ReaGroup ConstructReaGroup(
            ReaGroup reaGroup,
            List<ScheduleWeek> scheduleWeeks);

    }

    public class EntityConstructor : IEntityConstructor
    {
        private readonly Regex classNameRE = new(@"(?<=(<h5>))((\w+ *)+)");
        private readonly Regex classTypeRE = new(@"(?<=(<strong>))((\w+ *)+)");
        private readonly Regex classSubgroupRE = new(@"(?<=(data-subgroup=.))([a-z0-9A-Zа-яА-Я]+)");
        private readonly Regex dayOfWeekRE = new(@"(?<=(</strong>[\Wa-z]+))([а-я]+)");
        private readonly Regex dateRE = new(@"(?<=,)([0-9 а-я]+)(?=,)");
        private readonly Regex classOrdinalNumberRE = new(@"(?<=,.*)([0-9] [а-я]+)(?= *<br>)");
        private readonly Regex professorRE = new(@"(?<=\?q=)((\w+ *)+)");
        private readonly Regex buildingNumberRE = new(@"(?<=Аудитория:[\\a-z ]+)(\d+ \w+)");
        private readonly Regex AuditionRE = new(@"(?<=\w+ *-[\\n ]+)([0-9а-я//]+)");
        
        private List<ReaClass> ConstructReaClass(string classInfo)
        {
            List<ReaClass> reaClasses = new();
            // Checking if the class is combined
            var splittedInfo = classInfo.Split("element-info-body");

            for (int i = 1; i <= splittedInfo.Length; i++)
            {
                ReaClass reaClass = new()
                {
                    ClassName = classNameRE.Match(splittedInfo[i]).Value,

                    ClassType = classTypeRE.Match(splittedInfo[i]).Value,

                    OrdinalNumber = classOrdinalNumberRE.Match(splittedInfo[i]).Value,

                    Audition = buildingNumberRE.Match(splittedInfo[i]).Value + " - "
                                 + AuditionRE.Match(splittedInfo[i]).Value,

                    Professor = professorRE.Match(splittedInfo[i]).Value,

                    Subgroup = classSubgroupRE.Match(splittedInfo[i]).Value,
                };

                reaClasses.Add(reaClass);

            }

            return reaClasses;

        }

        public List<ScheduleWeek> ConstructScheduleWeeks(List<string> classInfoArray) 
        {
            var lastDayOfCurrentWeek = getDate(dateRE.Match(classInfoArray[0]).Value).GetWeekEnd();
            var scheduleWeekArray = new List<ScheduleWeek>() { 
                new ScheduleWeek() {Id = 0 } 
            };

            foreach (var classInfo in classInfoArray)
            {
                var classDate = getDate(dateRE.Match(classInfo).Value).GetWeekEnd();

                if (classDate.CompareTo(lastDayOfCurrentWeek) <= 0)
                    AddClassesToScheduleWeek(scheduleWeekArray.Last(), classInfo);

                else 
                { 
                    scheduleWeekArray.Last().WeekStart = lastDayOfCurrentWeek.GetWeekStart();
                    scheduleWeekArray.Last().WeekEnd = lastDayOfCurrentWeek.GetWeekEnd();
                    scheduleWeekArray.Add(
                        new ScheduleWeek() { Id = scheduleWeekArray.Last().Id + 1 }
                        );
                    //Add implementation of Hashing function
                    lastDayOfCurrentWeek = getDate(dateRE.Match(classInfo).Value).GetWeekEnd();
                }
            }

            return scheduleWeekArray;
        }

        private ScheduleWeek AddClassesToScheduleWeek(ScheduleWeek scheduleWeek, string classInfo)
        {
            var classDate = getDate(dateRE.Match(classInfo).Value);
            var dayOfWeek = dayOfWeekRE.Match(classInfo).Value.ToLower().Replace(" ", "");

            foreach (var prop in scheduleWeek.GetScheduleDays())
            {
                if (prop.DayOfWeekName == dayOfWeek) 
                {
                    prop.Date = classDate;
                    prop.ReaClasses.Concat(ConstructReaClass(classInfo));
                }
            }
            return scheduleWeek;
        }
        public ReaGroup ConstructReaGroup(
            ReaGroup reaGroup,
            List<ScheduleWeek> scheduleWeeks)
        {
            var newReaGroup = new ReaGroup() 
            {
                Id = 0, 
                GroupName = reaGroup.GroupName,
                ScheduleWeeks = scheduleWeeks                
            };

            return newReaGroup;

        }

        public static DateOnly getDate(string date)
        {
            DateOnly newDate = new();
            if (DateOnly.TryParse(date, out var realDate))
                return realDate;
            else 
                return newDate;
        }

    }
}
