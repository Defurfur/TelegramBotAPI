using ReaSchedule.Models;
using System.Text.RegularExpressions;

namespace ScheduleWorker
{
    public interface IEntityConstructor
    {
        List<ReaClass> ConstructReaClass(string modalInfo);
        ScheduleWeek ScheduleWeekConstructor(List<ReaClass> reaClasses);
        ReaGroup ReaGroupConstructor(List<ScheduleWeek> scheduleWeeks);

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
        
        public List<ReaClass> ConstructReaClass(string classInfo)
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

            //Поменять функцию джаваскрипта, чтобы она правильно возвращала айдишники сдвоенных уроков


        }

        public ScheduleWeek ConstructScheduleWeek(string[] classInfoArray)
        {
            var scheduleWeek = new ScheduleWeek() { Id = 0 };
            var randomCurrentWeekDate = dateRE.Match(classInfoArray[0]).Value;

            if (randomCurrentWeekDate == null)
                throw new ArgumentNullException("Date could not be found");

            var tryGetDate = DateOnly.TryParse(randomCurrentWeekDate, out var existingCurrentWeekDate);

            if (tryGetDate) { 
                scheduleWeek.WeekStart = existingCurrentWeekDate.GetWeekStart();
                scheduleWeek.WeekEnd = existingCurrentWeekDate.GetWeekEnd();
            }

            foreach (var classInfo in classInfoArray)
            {
                var dayOfWeek = dayOfWeekRE.Match(classInfo).Value.ToLower().Replace(" ", "");

                foreach (var prop in scheduleWeek.GetType().GetProperties().OfType<ScheduleDay>())
                {
                    prop.Date = existingCurrentWeekDate.GetDateByDayOfWeek(prop.DayOfWeekName);

                    if (prop.DayOfWeekName == dayOfWeek) 
                    {
                        prop.ReaClasses.Concat(ConstructReaClass(classInfo));
                    }
                }
            }

            return scheduleWeek;
            

        }
        public ReaGroup ReaGroupConstructor(List<ScheduleWeek> scheduleWeeks)
        {
            throw new NotImplementedException();
        }

    }
}
