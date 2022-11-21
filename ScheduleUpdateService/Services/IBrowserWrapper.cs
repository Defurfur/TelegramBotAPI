using PuppeteerSharp;
using System.Diagnostics;

namespace ScheduleUpdateService.Services
{
    public interface IBrowserWrapper : IAsyncDisposable
    {
        public Browser? Browser { get; set; }
        bool IsInit { get; set; }
        Task Init();
    }

    public class BrowserWrapper : IBrowserWrapper
    {
        public Browser? Browser { get; set; }
        public bool IsInit { get; set; }

        public async Task Init()
        {
            if (!IsInit)
            {
                Browser = await Puppeteer.LaunchAsync(
                    new LaunchOptions { 
                        Timeout = 300000,
                        Headless = true,
                        Args = new string[] { "--no-zygote", "--no-sandbox", "--single-process" } });
                IsInit = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (IsInit)
            {
                var pages = await Browser!.PagesAsync();
                if (pages != null)
                    foreach (var page in pages)
                        await page.CloseAsync();

                await Browser!.CloseAsync();
                await Browser!.DisposeAsync();

                KillChromiumProcesses();

                IsInit = false;
            }
        }

        private void KillChromiumProcesses()
        {
            Process[] processes = Process.GetProcessesByName("chrome");

            if(processes.Any() && processes != null)
                foreach (var process in processes)
                    process.Kill(true);

        }
    }
}

