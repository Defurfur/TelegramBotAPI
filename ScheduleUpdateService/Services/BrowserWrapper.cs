using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using ScheduleUpdateService.Abstractions;
using System.Diagnostics;
using XAct.Resources;
using XSystem.Security.Cryptography;

namespace ScheduleUpdateService.Services;



public class BrowserWrapper : IBrowserWrapper
{
    private readonly IConfiguration _config;

    public BrowserWrapper(IConfiguration config)
    {
        _config = config;
    }

    public Browser? Browser { get; set; }
    public bool IsInit { get; set; }

    public async Task Init()
    {
        if (!IsInit)
        {
            try
            {
                //using var browserFetcher = new BrowserFetcher();
                //var revisions = browserFetcher.LocalRevisions().ToList();
                //var canDownload = browserFetcher.CanDownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                //var something = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision);
                //var folder = browserFetcher.DownloadsFolder;

                Browser = await Puppeteer.LaunchAsync(
                new LaunchOptions {
                    ExecutablePath = _config.GetSection("ChromiumPaths").GetSection("DefaultChromium").Value,
                    Timeout = 300000,
                    Headless = true,
                    Args = new string[] { "--no-zygote", "--no-sandbox", "--single-process" } });

                IsInit = true;

            }
            catch (Exception ex)
            {

                throw new Exception("Could not initilize browser instance", ex);
            }
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

