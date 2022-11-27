using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleUpdateService.Services
{


    public class SimpleReaClassFactory : IReaClassFactory
    {
        #region Regex fields
        private readonly Regex _classNameRE = new(@"(?<=(<h5>))((\w+ *)+)");
        private readonly Regex _classTypeRE = new(@"(?<=(<strong>))((\w+ *)+)");
        private readonly Regex _classSubgroupRE = new(@"(?<=(data-subgroup=.))([a-z0-9A-Zа-яА-Я]+)");
        private readonly Regex _classOrdinalNumberRE = new(@"(\d{1}\s+пара)");
        private readonly Regex _professorRE = new(@"(?<=\?q=)((\w+ *)+)");
        private readonly Regex _dataElementIdRE = new(@"(?<=data-elementid=\S)(\d+)");
        private readonly Regex _auditionRe = new(@"(?<=Аудитория:\s*)([а-я/0-9/\s-]*)");
        #endregion
        public ReaClass CreateInstance(string classInfo)
        {
            var reaClass = new ReaClass()
            {
                ClassName = _classNameRE.Match(classInfo).Value,

                ClassType = _classTypeRE.Match(classInfo).Value,

                OrdinalNumber = _classOrdinalNumberRE.Match(classInfo).Value,

                Audition = _auditionRe.Match(classInfo).Value
                .Replace("\r", "")
                .Replace("\n", "")
                .Trim()
                .Replace("  ",""),

                Professor = _professorRE.Match(classInfo).Value,

                Subgroup = _classSubgroupRE.Match(classInfo).Value,

                ClassElementId = _dataElementIdRE.Match(classInfo).Value,
            };

            return reaClass;
        }

        public List<ReaClass> CreateFromList(IEnumerable<string> array)
        {
            var reaClasses = new List<ReaClass>();
            foreach (var text in array)
            {
                var cleanedString = CleanInputString(text);
                cleanedString.ForEach(x => reaClasses.Add(CreateInstance(x)));
            }

            return reaClasses;
        }

        private List<string> CleanInputString(string classInfo)
        {
            var delimeter = "element-info-body";
            var firstOccurenceOfDelimeter = classInfo.IndexOf(delimeter) + delimeter.Length;
            var cleanedClassInfoArray = classInfo
                .Substring(firstOccurenceOfDelimeter)
                .Split(delimeter)
                .ToList();
            return cleanedClassInfoArray;
        }
    }
}
