using ReaSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using AngleSharp.Dom;

namespace ScheduleWorker.Services
{
    public interface IGenericBuilder<T>
    {
        public T Build();
    }


    public sealed class ObjectFromStringBuilder<T> : IGenericBuilder<T>
        where T : class, new()
    {
        private Func<T, T>? _afterBuildActions;
        private Dictionary<string, Regex>? _propertyNameRegexDict;
        private string _parseSource = string.Empty;
        private string _splitString = string.Empty;

        public ObjectFromStringBuilder<T> ParseFrom(string parseSource)
        {
            _parseSource = parseSource;
            return this;
        }
        //<summary>
        //Method adds parsing dictionary to the builder. Parsing dictionary' keys must countain Tobject Properties' names
        //to parse info for. Values must cointain Regexes to parse the values for object properties. 
        //</summary>
        public ObjectFromStringBuilder<T> AddParsingDictionary(Dictionary<string, Regex> propertyNameRegexDict)
        {
            _propertyNameRegexDict = propertyNameRegexDict;
            return this;
        }
        //<summary>
        //Adds a split string to split the Parse Source. Use this with BuildMany method to return a list of objects     
        //</summary>
        public ObjectFromStringBuilder<T> AddSplitString(string splitString)
        {
            _splitString = splitString;
            return this;
        }

        private T ParseOne(string parseSource)
        {
            var tObject = new T();
            if (_propertyNameRegexDict is null || _parseSource == string.Empty)
                return tObject;

            foreach (KeyValuePair<string, Regex> pair in _propertyNameRegexDict)
            {
                var ObjectProperty = typeof(T).GetProperty(pair.Key);
                if (ObjectProperty == null)
                    continue;
                ObjectProperty.SetValue(tObject, pair.Value.Match(parseSource).Value);
            }

            return tObject;
        }

        private List<T> ParseMany()
        {
            List<T> newList = new();
            if (_propertyNameRegexDict is null || _parseSource == string.Empty)
            {
                newList.Add(new T());
                return newList;
            }

            if (_splitString == string.Empty)
            {
                newList.Add(ParseOne(_parseSource));
                return newList;
            }
            var splittedSource = _parseSource.Split(_splitString);
            foreach (var textPeice in splittedSource)
            {
                newList.Add(ParseOne(textPeice));
            }

            return newList;

        }

        public ObjectFromStringBuilder<T> AfterBuildActions(Func<T, T> actions)
        {
            _afterBuildActions = actions;

            return this;
        }


        public T Build()
        {
            var tObject = ParseOne(_parseSource);
            if (_afterBuildActions != null)
                _afterBuildActions(tObject);

            return tObject;
        }

        public List<T> BuildMany()
        {
            var listOfT = ParseMany();

            if (_afterBuildActions != null)
                foreach (var TObj in listOfT)
                {
                    _afterBuildActions(TObj);
                }

            return listOfT;
        }
    }

}