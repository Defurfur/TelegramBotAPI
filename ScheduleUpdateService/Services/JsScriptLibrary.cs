using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleUpdateService.Services;

public static class JsScriptLibrary
{
    public const string getScheduleFunc = """
        async function getSchedule(group, week)
        {
            var schedule;
            await $.ajax({
            url: '/Schedule/ScheduleCard', 
                    type: 'GET', 
                    data: { selection: group, weekNum: week, catfilter: 0 }, 
                    dataType: 'html', 
                    success: function(data) {
                    schedule = data;
                }
            }) 
                return schedule;
        };
        """;

    //!! Uncomment to get function for browser console !!

    //async function getSchedule(group, week)
    //{
    //    var schedule;
    //    await $.ajax({
    //    url: '/Schedule/ScheduleCard', 
    //            type: 'GET', 
    //            data: { selection: group, weekNum: week, catfilter: 0 }, 
    //            dataType: 'html', 
    //            success: function(data) {
    //            schedule = data;
    //        }
    //    }) 
    //        return schedule;
    //};

    public const string getWeeklyClassesById = """
        async function GetWeeklyClassesInfo(e){ 
            var modalInfo; 
            await $.ajax({ 
                url: '/Schedule/GetDetailsById', 
                type: 'GET', 
                data: {id: e }, 
                dataType: 'html', 
                success: function (data) { 
                    modalInfo = data; 
                } 
            }) 
        return modalInfo; 
        };
        """;

    //async function GetWeeklyClassesInfo(e){ 
    //        var modalInfo; 
    //        await $.ajax({ 
    //            url: '/Schedule/GetDetailsById', 
    //            type: 'GET', 
    //            data: {id: e }, 
    //            dataType: 'html', 
    //            success: function (data) { 
    //                modalInfo = data; 
    //            } 
    //        }) 
    //    return modalInfo; 
    //};

    public const string aggregateClassesInfoFunc = """
        async function aggregateClassesInfo(classList)
        {
            var classesInfo = [];
            if (!classList[0].includes(','))
            {
                for (let i = 0; i < classList.length; i++)
                {
                    var classInfo = await GetWeeklyClassesInfo(classList[i]);
                    classesInfo.push(classInfo);
                }
            }
            else
            {
                for (let j = 0; j < classList.length; j++)
                {
                    classList[j].split(', ');
                    var classInfo = await GetWeeklyClassesInfoByData(group, classList[j].split(', ')[0],
        classList[j].split(', ')[1]);
                    classesInfo.push(classInfo);
                }
            }
            return classesInfo;
        };
        """;

    //async function aggregateClassesInfo(classList)
    //{
    //    var classesInfo = [];
    //    if (!classList[0].includes(','))
    //    {
    //        for (let i = 0; i < classList.length; i++)
    //        {
    //            var classInfo = await GetWeeklyClassesInfo(classList[i]);
    //            classesInfo.push(classInfo);
    //        }
    //    }
    //    else
    //    {
    //        for (let j = 0; j < classList.length; j++)
    //        {
    //            classList[j].split(', ');
    //            var classInfo = await GetWeeklyClassesInfoByData(group, classList[j].split(', ')[0],
    //classList[j].split(', ')[1]);
    //            classesInfo.push(classInfo);
    //        }
    //    }
    //    return classesInfo;
    //};

    public const string getWeeklyClassesByData = """
        async function GetWeeklyClassesInfoByData(group, date, slot)
        {
            var modalInfo;
            await $.ajax({
            url: '/Schedule/GetDetails', 
                        type: 'GET', 
                        data: { selection: group, date: date, timeSlot: slot }, 
                        dataType: 'html', 
                        success: function(data) {
                    modalInfo = data;
                }
            }) 
                return modalInfo;
        };
        """;

    //!! Uncomment to get function for browser console !!

    //async function GetWeeklyClassesInfoByData(group, date, slot)
    //{
    //    var modalInfo;
    //    await $.ajax({
    //    url: '/Schedule/GetDetails', 
    //                type: 'GET', 
    //                data: { selection: group, date: date, timeSlot: slot }, 
    //                dataType: 'html', 
    //                success: function(data) {
    //            modalInfo = data;
    //        }
    //    }) 
    //        return modalInfo;
    //};

    public const string getClassesInfoByDataFunc =
        getScheduleFunc +
        getWeeklyClassesById +
        getWeeklyClassesByData +
        aggregateClassesInfoFunc +
        """
        async function getClassesInfoByDateAndSlot(group, week)
        {
            var schedule = await getSchedule(group, week);
            re = /(?<=\(\s&#39;)([^\)\"\>]*)/gm;
            dirtyArray = schedule.match(re);
            if (dirtyArray == null)
            {
                return "";
            }
            DateAndSlotArray = [];
            dirtyArray.forEach(e => { DateAndSlotArray.push(e.replaceAll('&#39;', ''))});
            var classesInfo = await aggregateClassesInfo(DateAndSlotArray);

            return classesInfo;

        }
        getClassesInfoByDateAndSlot(group, week);
        """;

    //async function getClassesInfoByDateAndSlot(group, week)
    //{
    //    var schedule = await getSchedule(group, week);
    //    re = / (?<=\(\s &#39;)([^\)\"\>]*)/gm;
    //    dirtyArray = schedule.match(re);
    //    if (dirtyArray == null)
    //    {
    //        return "";
    //    }
    //    DateAndSlotArray = [];
    //    dirtyArray.forEach(e => { DateAndSlotArray.push(e.replaceAll('&#39;', ''))});
    //    var classesInfo = await aggregateClassesInfo(DateAndSlotArray);

    //    return classesInfo;

    //}
    //await getClassesInfoByDateAndSlot(group, week);

    /// <summary>
    /// Returns a script as a string, which recieves a <paramref name="group"/>'s schedule of a certain 
    /// <paramref name="week"/> from rasp.rea.ru website as a list of strings. If there is no schedule 
    /// on a specific week, returns empty string.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="week"></param>
    /// <returns></returns>
    public static string GetClassesInfoByData(string group, int week)
    {
        string JsPipeline =
            $"group = '{group}';\r\n" +
            $"week = {week};\r\n" +
            getClassesInfoByDataFunc;

        return JsPipeline;
    }
    /// <summary>
    /// A script as a string, which checks whether the <paramref name="group"/> exists on rasp.rea.ru website or not.
    /// Script returns either true or false.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public static string CheckForGroupExistance(string group)
    {
        string func = "async function CheckForGropExistance(group)" +
            "\r\n" +
            "{" +
            "\r\n    let row;" +
            "\r\n    let ajaxData;" +
            "\r\n    await $.ajax({" +
            "\r\n                url: \"/Schedule/ScheduleCard\"," +
            "\r\n                type: \"GET\"," +
            "\r\n                data: { selection: group, weekNum: \"6\", catfilter: 0 }," +
            "\r\n                dataType: 'html'," +
            "\r\n                success: function (data) {" +
            "\r\n                ajaxData = data;" +
            "\r\n                }})" +
            "\r\n    row = ajaxData.includes(\"<div class=\")" +
            "\r\n    return row" +
            "\r\n}" +
            $"\r\ngroup = '{group}'" +
            "\r\nCheckForGropExistance(group)";



        return func;
    }
}
