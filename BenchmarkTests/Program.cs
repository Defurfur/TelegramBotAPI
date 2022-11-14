using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Jint.Native.Json;
using Newtonsoft.Json;
using ReaSchedule.Models;
using ScheduleWorker.Services;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("booba");
;//var builder = new BuilderBenchmarkTest();
//Console.WriteLine("result of ReflectionBuilder:" + $"{JsonConvert.SerializeObject(builder.CreateReaClassWithGenericAndReflectionUsed())}");
//Console.WriteLine("");
//Console.WriteLine("result of ReflectionBuilder:" + $"{JsonConvert.SerializeObject(builder.CreateReaClassWithFactory())}");
//Console.WriteLine("");
//Console.WriteLine("result of ReflectionBuilder:" + $"{JsonConvert.SerializeObject(builder.CreateReaClassesWithoutReflection2())}");
//Console.WriteLine("");
//Console.WriteLine("result of ReflectionBuilder:" + $"{JsonConvert.SerializeObject(builder.CreateReaClassesWithSelect())}");
//var value = BenchmarkRunner.Run<BuilderBenchmarkTest>();

//[MemoryDiagnoser]
//public class BuilderBenchmarkTest
//{

//    public BuilderBenchmarkTest()
//    {
//        var classInfo = _parseSource;

//        _delimeterString = "element-info-body";
//        _firstClassDelimeter = classInfo.IndexOf(_delimeterString) + _delimeterString.Length;
//        _cleanedClassInfo = classInfo.Substring(_firstClassDelimeter);
//        _splittedArray = _cleanedClassInfo.Split(_delimeterString);

//        _builder = new ObjectFromStringBuilder<ReaClass>()
//            .AddSplitString(_delimeterString)
//            .AddParsingDictionary(_propertyNameRegexDict)
//            .AfterBuildActions(x => { x.Audition.Replace("/r/n", ""); return x; })
//            .ParseFrom(_cleanedClassInfo);
//        _factory = new SimpleReaClassFactory();
//    }


//    private string _parseSource = "\"\r\n\r\n\r\n\r\n<a class=\"d-none element-direct-link\" href=\"/?id=2999942110,2999942110\">Ссылка на занятие</a>\r\n<div class=\"element-info-body\" data-elementid=\"2999942110\" data-subgroup=\"1\">\r\n    <h5>Элективные дисциплины по физической культуре и спорту</h5>\r\n    <strong>Практическое занятие</strong><br />\r\n    пятница, 21 октября 2022, 4 пара<br />\r\n\r\n        Аудитория: \r\n            2 корпус -\r\n        с/з\r\n        <br />\r\n            Площадка: Основная\r\n        <br />\r\n    <br />\r\n\r\n\r\n    Группа\r\n        15.02Д-ММ2/20б (1)\r\n    <br />\r\n    Базовая часть<br />\r\n\r\n    Преподаватель:\r\n    <br />\r\n        &emsp;&emsp;\r\n            <a href=\"?q=гаджиев далгат муратович\"><i class=\"material-icons\" style=\"vertical-align: middle\">school</i> Гаджиев Далгат Муратович</a>\r\n        <br />\r\n            &emsp;&emsp;\r\n            (Кафедра физического воспитания)\r\n            <br />\r\n        \r\n</div>\r\n    <hr />\r\n<div class=\"element-info-body\" data-elementid=\"2999942110\" data-subgroup=\"2\">\r\n    <h5>Элективные дисциплины по физической культуре и спорту</h5>\r\n    <strong>Практическое занятие</strong><br />\r\n    пятница, 21 октября 2022, 4 пара<br />\r\n\r\n        Аудитория: \r\n            2 корпус -\r\n        с/з\r\n        <br />\r\n            Площадка: Основная\r\n        <br />\r\n    <br />\r\n\r\n\r\n    Группа\r\n        15.02Д-ММ2/20б (2)\r\n    <br />\r\n    Базовая часть<br />\r\n\r\n    Преподаватель:\r\n    <br />\r\n        &emsp;&emsp;\r\n            <a href=\"?q=заппаров рустам илдарович\"><i class=\"material-icons\" style=\"vertical-align: middle\">school</i> Заппаров Рустам Илдарович</a>\r\n        <br />\r\n            &emsp;&emsp;\r\n            (Кафедра физического воспитания)\r\n            <br />\r\n        \r\n</div>\r\n    <hr />\r\n\"";
//    private string _cleanedClassInfo;
//    private ObjectFromStringBuilder<ReaClass> _builder;
//    private SimpleReaClassFactory _factory;
//    private string _delimeterString;
//    private int _firstClassDelimeter;
//    private string[] _splittedArray;
//    private readonly Dictionary<string, Regex> _propertyNameRegexDict = new()
//    {
//        {"ClassName", new Regex(@"(?<=(<h5>))((\w+ *)+)")},
//        {"ClassType", new Regex(@"(?<=(<strong>))((\w+ *)+)")},
//        {"OrdinalNumber", new Regex(@"(\d\s\w+)(?=<br\s/>)", RegexOptions.Singleline)},
//        {"Audition", new Regex(@"(?<=Аудитория:\s*)(\d+\s\w+\s*-\s+[0-9а-я/]+)")},
//        {"Professor", new Regex(@"(?<=\?q=)((\w+ *)+)")},
//        {"Subgroup", new Regex(@"(?<=(data-subgroup=.))([a-z0-9A-Zа-яА-Я]+)")},
//        {"ClassElementId", new Regex(@"(?<=data-elementid=\S)(\d+)")},
//    };

//    [Benchmark]
//    public List<ReaClass> CreateReaClassWithGenericAndReflectionUsed()
//    {
//        return _builder.BuildMany();
//    }


//    [Benchmark]
//    public List<ReaClass> CreateReaClassWithFactory()
//    {
//        var reaClasses = new List<ReaClass>();

//        foreach(var text in _splittedArray)
//        {
//            reaClasses.Add(_factory.CreateInstance(text));
//        }

//        return reaClasses;
//    }

//    //[Benchmark]
//    //public List<ReaClass> CreateReaClassesWithoutReflection2()
//    //{
//    //    var classInfo = _parseSource;

//    //    var delimeterString = "element-info-body";
//    //    var firstClassDelimeter = classInfo.IndexOf(delimeterString) + delimeterString.Length;
//    //    var cleanedClassInfo = classInfo.Substring(firstClassDelimeter);
//    //    var reaClassBuilder = new SimpleReaClassBuilder();
//    //    var reaClasses = new List<ReaClass>();

//    //    foreach(var text in cleanedClassInfo.Split(delimeterString))
//    //    {
//    //        reaClassBuilder.AddParseSource(text);
//    //        var reaClass = reaClassBuilder.Build();
//    //        reaClasses.Add(reaClass);
//    //    }
//    //    return reaClasses;
//    //}
//    //[Benchmark]
//    //public List<ReaClass> CreateReaClassesWithSelect()
//    //{
//    //    var classInfo = _parseSource;

//    //    var delimeterString = "element-info-body";
//    //    var firstClassDelimeter = classInfo.IndexOf(delimeterString) + delimeterString.Length;
//    //    var cleanedClassInfo = classInfo.Substring(firstClassDelimeter).Split(delimeterString);

//    //    var reaClassBuilder = new SimpleReaClassBuilder();
//    //    List<ReaClass> classArray = cleanedClassInfo
//    //        .Select(x => reaClassBuilder.AddParseSource(x).Build())
//    //        .ToList();

//    //    return classArray;
//    //}


//}

