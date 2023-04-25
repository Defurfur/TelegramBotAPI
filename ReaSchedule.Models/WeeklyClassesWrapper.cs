using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaSchedule.Models
{
    public struct WeeklyClassesWrapper
    {
        public List<string> WeeklyClasses { get; set; }
        public int WeekNumber { get; set; }
        public WeeklyClassesWrapper(List<string> weeklyClasses, int weekNumber)
        {
            WeeklyClasses = weeklyClasses;
            WeekNumber = weekNumber;
        }
        public void Deconstruct(out List<string> weeklyClasses, out int weekNumber)
        {
            weeklyClasses = WeeklyClasses;
            weekNumber = WeekNumber;
        }
    }
}
