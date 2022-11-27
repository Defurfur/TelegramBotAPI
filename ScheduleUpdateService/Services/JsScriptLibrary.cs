using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleUpdateService.Services;

public static class JsScriptLibrary
{
    public const string getScheduleFunc =
        "async function getSchedule(group, week) {" + 
        "\r\n    var schedule;" +
        "\r\n    await $.ajax({" +
        "\r\n        url: \"/Schedule/ScheduleCard\"," +
        "\r\n        type: \"GET\"," +
        "\r\n        data: { selection: group, weekNum: week, catfilter: 0 }," +
        "\r\n        dataType: 'html'," +
        "\r\n        success: function (data) {" +
        "\r\n           schedule = data;" +
        "\r\n            }" +
        "\r\n        })" +
        "\r\n    return schedule;" +
        "\r\n}";

    public const string getWeeklyClassesById =
        "async function GetWeeklyClassesInfo(e){" +
        "\r\n        var modalInfo;" +
        "\r\n        await $.ajax({" +
        "\r\n            url: \"/Schedule/GetDetailsById\"," +
        "\r\n            type: \"GET\"," +
        "\r\n            data: {id: e }," +
        "\r\n            dataType: 'html'," +
        "\r\n            success: function (data) {" +
        "\r\n                modalInfo = data;" +
        "\r\n            }" +
        "\r\n        })" +
        "\r\n    return modalInfo;" +
        "\r\n}";
    public const string aggregateClassesInfoFunc =
        "async function aggregateClassesInfo(classList){" +
        "\r\n    var classesInfo = []; " +
        "\r\n    if (!classList[0].includes(',')){" +
        "\r\n        for (let i = 0; i < classList.length; i++){" +
        "\r\n            var classInfo = await GetWeeklyClassesInfo(classList[i]);" +
        "\r\n            classesInfo.push(classInfo);" +
        "\r\n        }" +
        "\r\n    }" +
        "\r\n    else {" +
        "\r\n        for (let j = 0; j < classList.length; j++){" +
        "\r\n            classList[j].split(\", \");" +
        "\r\n            var classInfo = await GetWeeklyClassesInfoByData(group, classList[j].split(\", \")[0], " +
        "classList[j].split(\", \")[1]);" +
        "\r\n            classesInfo.push(classInfo);" +
        "\r\n        }" +
        "\r\n    }" +
        "\r\n    return classesInfo;" +
        "\r\n}";

    public const string getWeeklyClassesByData =
        "async function GetWeeklyClassesInfoByData(group, date, slot){" +
        "\r\n        var modalInfo;" +
        "\r\n        await $.ajax({" +
        "\r\n            url: \"/Schedule/GetDetails\"," +
        "\r\n            type: \"GET\"," +
        "\r\n            data: { selection: group, date: date, timeSlot: slot }," +
        "\r\n            dataType: 'html'," +
        "\r\n            success: function (data) {" +
        "\r\n                modalInfo = data;" +
        "\r\n            }" +
        "\r\n        })" +
        "\r\n    return modalInfo;" +
        "\r\n}";

    public const string getClassesInfoByDataFunc =
        getScheduleFunc +
        getWeeklyClassesById +
        getWeeklyClassesByData +
        aggregateClassesInfoFunc +
        "async function getClassesInfoByDateAndSlot(group, week){" +
        "\r\n    var schedule = await getSchedule(group, week);" +
        "\r\n    re = /(?<=&#39;)([0-9.]+&#39;, *&#39;\\d)/gm;" +
        "\r\n    dirtyArray = schedule.match(re);" +
        "\r\n    DateAndSlotArray = [];" +
        "\r\n    dirtyArray.forEach(e => {DateAndSlotArray.push(e.replaceAll(\"&#39;\", \"\"))});" +
        "\r\n    var classesInfo = await aggregateClassesInfo(DateAndSlotArray);" +
        "\r\n    " +
        "\r\n    return classesInfo;" +
        "\r\n" +
        "}\r\n" +
        "await getClassesInfoByDateAndSlot(group, week);";

    public static string getClassesInfoByData(string group, int week)
    {
        string JsPipeline =
            $"group = '{group}';\r\n" +
            $"week = {week};\r\n" +
            getScheduleFunc +
            getWeeklyClassesById +
            getWeeklyClassesByData +
            aggregateClassesInfoFunc +
            "\r\nasync function getClassesInfoByDateAndSlot(group, week){" +
            "\r\n    var schedule = await getSchedule(group, week);" +
            "\r\n    re = /(?<=&#39;)([0-9.]+&#39;, *&#39;\\d)/gm;" +
            "\r\n    dirtyArray = schedule.match(re);" +
            "\r\n    if(dirtyArray == null)" +
            "\r\n       return '';" +
            "\r\n    DateAndSlotArray = [];" +
            "\r\n    dirtyArray.forEach(e => {DateAndSlotArray.push(e.replaceAll(\"&#39;\", \"\"))});" +
            "\r\n    var classesInfo = await aggregateClassesInfo(DateAndSlotArray);" +
            "\r\n    " +
            "\r\n    return classesInfo;" +
            "\r\n" +
            "}\r\n" +
            "\r\n getClassesInfoByDateAndSlot(group, week)";

        return JsPipeline;
    }

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
