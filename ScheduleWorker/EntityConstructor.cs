using ReaSchedule.Models;
using System.Text.RegularExpressions;

namespace ScheduleWorker
{
    public interface IEntityConstructor
    {
        List<ReaClass> ReaClassConstructor(string modalInfo);
        ScheduleWeek ScheduleWeekConstructor(List<ReaClass> reaClasses);
        ReaGroup ReaGroupConstructor(List<ScheduleWeek> scheduleWeeks);

    }

    public class EntityConstructor : IEntityConstructor
    {
        private readonly Regex classNameRE = new(@"(?<=(<h5>))((\w+ *)+)");
        private readonly Regex classTypeRE = new(@"(?<=(<strong>))((\w+ *)+)");
        private readonly Regex classSubgroupRE = new(@"(?<=(data-subgroup=.))([a-z0-9A-Zа-яА-Я]+)");
        private readonly Regex dayOfTheWeekRE = new(@"(?<=(</strong>[\Wa-z]+))([а-я]+)");
        private readonly Regex dateRE = new(@"(?<=,)([0-9 а-я]+)(?=,)");
        private readonly Regex classOrdinalNumberRE = new(@"(?<=,.*)([0-9] [а-я]+)(?= *<br>)");
        private readonly Regex professorRE = new(@"(?<=\?q=)((\w+ *)+)");
        private readonly Regex buildingNumberRE = new(@"(?<=Аудитория:[\\a-z ]+)(\d+ \w+)");
        private readonly Regex AuditionRE = new(@"(?<=\w+ *-[\\n ]+)([0-9а-я//]+)");
        
        public List<ReaClass> ReaClassConstructor(string classInfo)
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

        public ReaGroup ReaGroupConstructor(List<ScheduleWeek> scheduleWeeks)
        {
            throw new NotImplementedException();
        }

        public ScheduleWeek ScheduleWeekConstructor(List<ReaClass> reaClasses)
        {
            throw new NotImplementedException();
        }
    }
}
