using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleWorker.Services
{

    public interface IParserPipeline
    {
        int WeekCountToParse { get; set; }
        Task<ReaGroup> ParseAndUpdate(ReaGroup reaGroup);
    }
    public class ParserPipeline : IParserPipeline
    {
        private readonly IScheduleLoader _scheduleLoader;
        private readonly IEntityUpdater _entityUpdater;
        public int WeekCountToParse { get; set; } = 2;

        public ParserPipeline(
            IScheduleLoader scheduleLoader,
            IEntityUpdater entituUpdater)
        {
            _scheduleLoader = scheduleLoader;
            _entityUpdater = entituUpdater;
            WeekCountToParse = WeekCountToParse;
        }


        public async Task<ReaGroup> ParseAndUpdate(ReaGroup reaGroup)
        {
            if (WeekCountToParse < 1)
                throw new Exception("Cannot parse 0 or less weeks");

            var listOfWeekClassWrappers = await _scheduleLoader.LoadPageContentAndParse(WeekCountToParse, reaGroup);

            ReaGroup updatedReaGroup = _entityUpdater.UpdateReaGroup(reaGroup, listOfWeekClassWrappers);


            return updatedReaGroup;
        }

        public void SetWeekCount(int weekCount)
        {
            WeekCountToParse = weekCount;
        }
    }
}
