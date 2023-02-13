﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using PuppeteerSharp;
using ReaSchedule.Models;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Extensions;
using System.Diagnostics;

namespace ScheduleUpdateService.Services;

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
public class JsScheduleParser : IScheduleParser
{
    private readonly IAsyncPolicy<JToken> _retryPolicy =
        Policy<JToken>
        .Handle<IOException>()
        .Or<PuppeteerException>()
        .WaitAndRetryAsync(2, x => TimeSpan.FromSeconds(5));
        

    private readonly ILogger<JsScheduleParser> _logger;
    private readonly IBrowserWrapper _browserWrapper;
    private readonly NavigationOptions _navigationOptions = new()
    {   Timeout = 0,
        WaitUntil = new[] { WaitUntilNavigation.Load } };
    private readonly string _reaWebsiteLink = "https://rasp.rea.ru/";

    public JsScheduleParser(
        IBrowserWrapper browserWrapper,
        ILogger<JsScheduleParser> logger)
    {
        _browserWrapper = browserWrapper;
        _logger = logger;
    }
    private async Task<JToken> GetJsomFromPage(IPage page, int weekNumber, string groupName)
    {
        string script = JsScriptLibrary.GetClassesInfoByData(groupName, weekNumber);

        page.Error += (sender, errorArgs) =>
        {
            _logger.LogWarning(
                "Evaluating script failed with error: {error}",
                errorArgs.Error);

            throw new PuppeteerException($"Page crashed before an attempt to parse data with following error: {errorArgs.Error}");
        };

        page.PageError += (sender, errorArgs) =>
        {
            _logger.LogWarning(
                "Evaluating script failed with error: {error}",
                errorArgs.Message);

            throw new PuppeteerException($"Page crashed before an attempt to parse data with following error: {errorArgs.Message}");

        };

        var jToken = await _retryPolicy
            .ExecuteAsync(() => page.EvaluateExpressionAsync(script));

        ArgumentNullException.ThrowIfNull(jToken);

        //var jToken = await page.WaitForExpressionAsync(script, new WaitForFunctionOptions() { Timeout = 0});

        return jToken;
    }
    private async Task<List<string>> GetJsonAndParse(IPage page, int weekNumber, string groupName)
    {
        List<string> weeklyClassesList = new();

        var jToken = await _retryPolicy
            .ExecuteAsync( () => GetJsomFromPage(page, weekNumber, groupName));

        if (jToken.ToString() == "")
        {
            return weeklyClassesList;
        }

        foreach (var classInfo in jToken)
        {
            weeklyClassesList.Add(classInfo.ToString());
        }

        return weeklyClassesList;
    }
    private async Task<List<WeeklyClassesWrapper>> ParsePageContent(IPage page, int weekCountToParse, string groupName)
    {
        var allWeeklyClasses = new List<WeeklyClassesWrapper>();

        for (int i = 1; i <= weekCountToParse; i++)
        {
            int weekNumber = DateTime.Now.GetWeekNumber() + i - 1;

            WeeklyClassesWrapper weeklyClassesWrapper = new();

            var weeklyClassesList = await GetJsonAndParse(page, weekNumber, groupName);

            weeklyClassesWrapper.WeeklyClasses = weeklyClassesList;
            weeklyClassesWrapper.WeekNumber = weekNumber;

            allWeeklyClasses.Add(weeklyClassesWrapper);
        }

        await page.CloseAsync();

        return allWeeklyClasses;
    }

    public async Task<List<WeeklyClassesWrapper>> LoadPageContentAndParse(
        int weekCountToParse,
        ReaGroup reaGroup)
    {
        IPage? page = null;

        var url = _reaWebsiteLink + "?q=" + reaGroup.GroupName.Replace("/", "%2F");

        try
        {
            page = await LoadPageContent(url);
        }
        catch(Exception ex)
        {
            _logger.LogError(
                ex,
                "{exName} has been thrown during {task}",
                ex.GetType().Name,
                nameof(LoadPageContentAndParse));
        }

        Debug.Assert(page != null, "[JsScheduleParser] could not download page content");

        var result = await ParsePageContent(page, weekCountToParse, reaGroup.GroupName);

        return result;
    }

    public async Task<bool> CheckForGroupExistance(string groupName)
    {
        var url = _reaWebsiteLink + "?q=" + groupName.Replace("/", "%2F");

        if (!_browserWrapper.IsInit)
            await _browserWrapper.InitAsync(CancellationToken.None);

        ArgumentNullException.ThrowIfNull(_browserWrapper.Browser);

        await using var page = await _browserWrapper.Browser!.NewPageAsync();

        await page.GoToAsync(url, _navigationOptions);
        await page.WaitForNavigationAsync(_navigationOptions);

        string script = JsScriptLibrary.CheckForGroupExistance(groupName);

        var jToken = await page.EvaluateExpressionAsync(script);

        var exists = Convert.ToBoolean(jToken.ToString());

        return exists;
    }

    private async Task<IPage> LoadPageContent(string url)
    {

        if (!_browserWrapper.IsInit)
            await _browserWrapper.InitAsync(CancellationToken.None);

        ArgumentNullException.ThrowIfNull(_browserWrapper.Browser);

        var page = await _browserWrapper.Browser!.NewPageAsync();

        await page.GoToAsync(url, _navigationOptions);

        await page.WaitForNavigationAsync(_navigationOptions);

        return page;
        


    }
}