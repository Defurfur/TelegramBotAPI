using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleWorker
{

    public interface IParserPipeline 
    {
        int weekCountToParse { get; set; }
        Task<ReaGroup> Execute(ReaGroup reaGroup);
    }
    public class ParserPipeline : IParserPipeline
    {
        private readonly IScheduleLoader _scheduleLoader;
        private readonly IEntityConstructor _entityConstructor;
        public int weekCountToParse { get; set; } = 2;

        public ParserPipeline(
            IScheduleLoader scheduleLoader,
            IEntityConstructor entityConstructor)
        {
            _scheduleLoader = scheduleLoader;
            _entityConstructor = entityConstructor;
            this.weekCountToParse = weekCountToParse;
        }


        public async Task<ReaGroup> Execute(ReaGroup reaGroup) 
        {
            if (weekCountToParse < 1)
                throw new Exception("Cannot parse 0 or less weeks");

            var classesInfoArray = await _scheduleLoader.LoadPageContentAndParse( weekCountToParse, reaGroup);

            ReaGroup newReaGroup = _entityConstructor.ConstructReaGroup(
                 reaGroup, 
                 _entityConstructor.ConstructScheduleWeeks(classesInfoArray)
                 );
            

            return newReaGroup;
        }

        public void SetWeekCount(int weekCount) 
        {
            weekCountToParse = weekCount;
        }
    }
}
