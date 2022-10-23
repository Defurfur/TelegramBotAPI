using AngleSharp.Dom;
using PuppeteerSharp;
using ReaSchedule.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleWorker
{
    public class InitialDbUpdater
    {
        private readonly IBrowserWrapper _browserWrapper;
        private readonly ScheduleDbContext _context;
        private readonly string reaWebsiteLink = "https://rasp.rea.ru/";

        public InitialDbUpdater(
            IBrowserWrapper browserWrapper,
            ScheduleDbContext context)
        {
            _browserWrapper = browserWrapper;
            _context = context;
        }

        public async Task InitialDbUpdate(Page page)
        {
            await GetGroupNames();
        }

        private async Task<List<string>> GetGroupNames()
        {
            var groupNames = new List<string>();

            if (!_browserWrapper.isInit)
                await _browserWrapper.Init();

            await using var page = await _browserWrapper.browser!.NewPageAsync();

            await page.GoToAsync(reaWebsiteLink);

            await page.WaitForNavigationAsync(new NavigationOptions() {Timeout = 0 });

            await page.EvaluateExpressionAsync("document.getElementById('expanderGroup').click()");

            var facultyNodes = await page.EvaluateExpressionAsync(
                "C = document.querySelector" +
                "('#groupControls > .col-sm-12 > select').childNodes;" +
                "\r\nC.forEach(e => {" +
                "\r\n    if (e.nodeName == '#text'){" +
                "\r\n        e.parentNode.removeChild(e)" +
                "\r\n    }" +
                "\r\n});" +
                "\r\nC");
            foreach (var facultyNode in facultyNodes)
            {

            }
        }
    }
}
