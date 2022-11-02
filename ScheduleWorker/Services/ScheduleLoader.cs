using PuppeteerSharp;
using ReaSchedule.Models;

namespace ScheduleWorker.Services;


public interface IScheduleLoader
{
    Task<List<WeeklyClassesWrapper>> LoadPageContentAndParse(
        int weekCountToParse,
        ReaGroup reaGroup);
}

//public class PuppeteerScheduleLoader : IScheduleLoader
//{
//    private readonly IBrowserWrapper _browserWrapper;
//    private readonly NavigationOptions navigationOptions = new() { Timeout = 0 };
//    private readonly string reaWebsiteLink = "https://rasp.rea.ru/";

//    public PuppeteerScheduleLoader(IBrowserWrapper browserWrapper)
//    {
//        _browserWrapper = browserWrapper;
//    }


//    public async Task<List<string>> LoadPageContentAndParse(ReaGroup reaGroup)
//    {

//        var url = reaWebsiteLink + "?q=" + reaGroup.GroupName.Replace("/", "%2F");

//        await using var page = await LoadPageContent(url);

//        var classInfoArray = await LoadButtonsAndClick(page);

//        return classInfoArray;

//    }

//    private async Task<Page> LoadPageContent(string url)
//    {
//        if (!_browserWrapper.isInit)
//            await _browserWrapper.Init();


//        await using var page = await _browserWrapper.browser!.NewPageAsync();

//        await page.GoToAsync(url);

//        await page.WaitForNavigationAsync(navigationOptions);

//        return page;
//    }
//    private async Task<List<ElementHandle>> LoadWeeklyClassesAsync(Page page)
//    {

//        var dayColumns = await page.QuerySelectorAllAsync("#zoneTimetable > .row > .col-lg-6 > " +
//            ".container > .table > tbody ");

//        List<ElementHandle> buttonLinks = new();

//        foreach (var column in dayColumns)
//        {
//            var classSlots = await column.QuerySelectorAllAsync(".slot");

//            foreach (ElementHandle classSlot in classSlots)
//            {
//                string styleInfo = await classSlot.EvaluateFunctionAsync<string>("e => e.getAttribute('style')");

//                if (styleInfo != "background-color: #f3f3f3" & styleInfo != null)
//                {
//                    buttonLinks.Add(await classSlot.QuerySelectorAsync("td > .task"));
//                } 
//            }
//        }

//        return buttonLinks;

//    }

//    private async Task<List<string>> LoadButtonsAndClick(Page page)
//    {
//        var classLinks = await LoadWeeklyClassesAsync(page);
//        var classInfoArray = new List<string>();

//        foreach(ElementHandle classLink in classLinks)
//        {
//            await classLink.EvaluateFunctionAsync("e => e.click()");

//            var textInfo = await page.WaitForSelectorAsync("#upTimeslotInfo > div " +
//                    "> div > div > .element-info-body");
//            if (textInfo == null)
//                throw new Exception("Modal window content didn't load in time");

//            var modalText = await page.EvaluateExpressionAsync("document.querySelector" +
//                "('#upTimeslotInfo > div > div > .modal-body').innerHTML");

//            classInfoArray.Append(modalText.ToString());

//            var closeButton = await page.QuerySelectorAsync(".modal-content > .modal-header >" +
//                " .rea-modal-button-block > button");

//            await closeButton.EvaluateFunctionAsync("e => e.click()");
//        }

//        return classInfoArray;
//    }

////}
public class JsScheduleLoader : IScheduleLoader
{
    private readonly IBrowserWrapper _browserWrapper;
    private readonly NavigationOptions _navigationOptions = new() { Timeout = 0 };
    private readonly string _reaWebsiteLink = "https://rasp.rea.ru/";

    public JsScheduleLoader(IBrowserWrapper browserWrapper)
    {
        _browserWrapper = browserWrapper;
    }
    public async Task<List<WeeklyClassesWrapper>> LoadPageContentAndParse(
        int weekCountToParse,
        ReaGroup reaGroup)
    {
        var url = _reaWebsiteLink + "?q=" + reaGroup.GroupName.Replace("/", "%2F");
        await using var page = await LoadPageContent(url);
        var allWeeklyClasses = new List<WeeklyClassesWrapper>();

        for (int i = 1; i <= weekCountToParse; i++)
        {
            int weekNumber = DateTime.Now.GetWeekNumber() + i - 1;
            WeeklyClassesWrapper weeklyClassesWrapper = new();
            List<string> weeklyClassesList = new();

            var classesInfoJToken = await page.EvaluateExpressionAsync
                (JsScriptLibrary.getClassesInfoByData(
                    reaGroup.GroupName,
                    weekNumber));

            if (classesInfoJToken.ToString() == "")
            {
                weeklyClassesWrapper.WeekNumber = weekNumber;
                weeklyClassesWrapper.WeeklyClasses = weeklyClassesList;

                allWeeklyClasses.Add(weeklyClassesWrapper);
                continue;
            }
            
            foreach (var classInfo in classesInfoJToken)
                weeklyClassesList.Add(classInfo.ToString());

            weeklyClassesWrapper.WeeklyClasses = weeklyClassesList;
            weeklyClassesWrapper.WeekNumber = weekNumber;

            allWeeklyClasses.Add(weeklyClassesWrapper);
        }

        return allWeeklyClasses;

    }
    private async Task<Page> LoadPageContent(string url)
    {
        if (!_browserWrapper.IsInit)
            await _browserWrapper.Init();


        var page = await _browserWrapper.Browser!.NewPageAsync();

        await page.GoToAsync(url);

        await page.WaitForNavigationAsync(_navigationOptions);

        return page;
    }
}