using PuppeteerSharp;
using ReaSchedule.Models;

namespace ScheduleWorker;

public interface IBrowserWrapper : IDisposable
{
    public Browser? browser { get; set; }
    bool isInit { get; set; }
    Task Init();
}
public interface IScheduleLoader
{
    Task LoadPageContentAndParse(
        string url,
        Func<string, Task> parseActions);
}
public class BrowserWrapper : IBrowserWrapper
{
    public Browser? browser { get; set; } = null;
    public bool isInit { get; set; } = false;

    public async Task Init()
    {
        if (!isInit)
        {
            browser = await Puppeteer.LaunchAsync(
                new LaunchOptions { Headless = true });
            isInit = true;
        }   
    }
    public void Dispose()
    {
        if (isInit)
        {
            browser!.Dispose();
            if (browser.IsClosed == true)
            {
                isInit = false;
            }
            else {
                throw new Exception("Could not dispose browser instance");
            }
        }
    }

} 
public class PuppeteerScheduleLoader : IScheduleLoader
{
    private readonly IBrowserWrapper _browserWrapper;
    private readonly NavigationOptions navigationOptions = new() { Timeout = 0 };
    private readonly string reaWebsiteLink = "https://rasp.rea.ru/";

    public PuppeteerScheduleLoader(IBrowserWrapper browserWrapper)
    {
        _browserWrapper = browserWrapper;
    }


    public async Task LoadPageContentAndParse(
        string url, 
        Func<string, Task> parseActions)
    {


        await using var page = await LoadPageContent(url);

        await LoadButtonsAndClick(page, parseActions);

    }

    private async Task<Page> LoadPageContent(string url)
    {
        if (!_browserWrapper.isInit)
            await _browserWrapper.Init();


        await using var page = await _browserWrapper.browser!.NewPageAsync();

        await page.GoToAsync(url);

        await page.WaitForNavigationAsync(navigationOptions);

        return page;
    }
    private async Task<List<ElementHandle>> LoadWeeklyClassesAsync(Page page)
    {

        var dayColumns = await page.QuerySelectorAllAsync("#zoneTimetable > .row > .col-lg-6 > " +
            ".container > .table > tbody ");

        List<ElementHandle> buttonLinks = new();

        foreach (var column in dayColumns)
        {
            var classSlots = await column.QuerySelectorAllAsync(".slot");

            foreach (ElementHandle classSlot in classSlots)
            {
                string styleInfo = await classSlot.EvaluateFunctionAsync<string>("e => e.getAttribute('style')");

                if (styleInfo != "background-color: #f3f3f3" & styleInfo != null)
                {
                    buttonLinks.Add(await classSlot.QuerySelectorAsync("td > .task"));
                } 
            }
        }

        return buttonLinks;

    }

    private async Task LoadButtonsAndClick(
        Page page, 
        Func<string, Task> parseActions)
    {
        var classLinks = await LoadWeeklyClassesAsync(page);

        foreach(ElementHandle classLink in classLinks)
        {
            await classLink.EvaluateFunctionAsync("e => e.click()");

            var textInfo = await page.WaitForSelectorAsync("#upTimeslotInfo > div " +
                    "> div > div > .element-info-body");
            if (textInfo == null)
                throw new Exception("Modal window content didn't load in time");

            var modalText = await page.EvaluateExpressionAsync("document.querySelector" +
                "('#upTimeslotInfo > div > div > .modal-body').innerHTML");

            await parseActions(modalText.ToString());

            var closeButton = await page.QuerySelectorAsync(".modal-content > .modal-header >" +
                " .rea-modal-button-block > button");

            await closeButton.EvaluateFunctionAsync("e => e.click()");
        }
    }

}
public class JsScheduleLoader : IScheduleLoader
{
    private readonly IBrowserWrapper _browserWrapper;
    private readonly NavigationOptions navigationOptions = new() { Timeout = 0 };
    private readonly string reaWebsiteLink = "https://rasp.rea.ru/";

    public JsScheduleLoader(IBrowserWrapper browserWrapper)
    {
        _browserWrapper = browserWrapper;
    }
    public async Task LoadPageContentAndParse(
        ReaGroup reaGroup,
        Func<string, Task> parseActions)
    {
        var url = reaWebsiteLink + "?q=" + reaGroup.GroupName.Replace("/", "%2F");
        await using var page = await LoadPageContent(url);
        var classInfoArray = await page.EvaluateExpressionAsync(
            JsScriptLibrary.getClassesInfoByData(reaGroup.GroupName, DateTime.Now.GetWeekNumber()));

        foreach (var classInfo in classInfoArray)
        {
            await parseActions(classInfo.ToString());
        }

    }
    private async Task<Page> LoadPageContent(string url)
    {
        if (!_browserWrapper.isInit)
            await _browserWrapper.Init();


        await using var page = await _browserWrapper.browser!.NewPageAsync();

        await page.GoToAsync(url);

        await page.WaitForNavigationAsync(navigationOptions);

        return page;
    }
}