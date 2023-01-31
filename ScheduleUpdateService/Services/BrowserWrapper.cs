using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using ScheduleUpdateService.Abstractions;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;
using XAct.Resources;
using XSystem.Security.Cryptography;

namespace ScheduleUpdateService.Services;



public class BrowserWrapper : IBrowserWrapper
{
    private readonly ILogger<BrowserWrapper> _logger;
    private readonly IChromiumKiller _chromiumKiller;
    private string _chromiumPath = string.Empty;
    private string _defaultChromiumPath = string.Empty;
    private string _webSocketEndpoint = string.Empty;
    public IBrowser? Browser { get; private set; }
    public bool IsInit { get; private set; }
    public bool IsBeingInitialized { get; private set; }


    public BrowserWrapper(
        ILogger<BrowserWrapper> logger,
        IChromiumKiller chromiumKiller,
        IConfiguration config)
    {
        _defaultChromiumPath = config
            .GetSection("ChromiumPaths").GetSection("DefaultChromium").Value ?? string.Empty;
        _logger = logger;
        _chromiumKiller = chromiumKiller;
    }

    public async ValueTask InitAsync(CancellationToken ct)
    {
        if (IsInit && !IsBeingInitialized && Browser is not null)
        {
            return;
        }

        try
        {
            if(!IsInit && !IsBeingInitialized)
            {
                IsBeingInitialized = true;

                IsInit = await TryConnectToBrowserAsync(ct);

                IsBeingInitialized = false;
            }
            else
            {
                while(!IsInit && IsBeingInitialized && !ct.IsCancellationRequested)
                {
                    await Task.Delay(100, ct);
                }

                ct.ThrowIfCancellationRequested();

                return;
            }

        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "[BrowserWrapper] Didn't manage to initialize browser.");
        }

    }

    public async ValueTask DisposeAsync()
    {
        if (!IsInit)
        {
            return;
        }

        try
        {
            var pages = await Browser!.PagesAsync();

            if (pages != null)
                await Task.WhenAll(pages.Select(x => x.CloseAsync()));

            await Browser.CloseAsync();

            await Browser.DisposeAsync();

        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "[BrowserWrapper] {exName} occured while an attempt to dispose browser",
                ex.GetType().Name);
        }
        finally
        {
            _chromiumKiller.KillChromiumProcesses(_chromiumPath);
            IsInit = false;
        }


    }

    private async ValueTask<bool> TryConnectToBrowserAsync(CancellationToken ct)
    {
        if (_webSocketEndpoint == string.Empty)
        {
            return await TryLaunchBrowser(ct);
        }

        Browser = await Puppeteer.ConnectAsync(new ConnectOptions()
        {
            BrowserWSEndpoint = _webSocketEndpoint,
        });


        if (Browser.IsConnected)
        {
            return true;
        }
        else
        {
            if (_chromiumPath == string.Empty)
                _chromiumKiller.KillChromiumProcesses(_defaultChromiumPath);
            else
                _chromiumKiller.KillChromiumProcesses(_chromiumPath);

            return await TryLaunchBrowser(ct);
        }
    }
    // Implement usage of defaultChromium instead of chromium path
    private async ValueTask<bool> TryLaunchBrowser(CancellationToken ct)
    {

        var path = await DownloadChromiumIfNeeded(ct);

        _chromiumPath = 
            path == string.Empty
            ? _defaultChromiumPath
            : path;

        if (_chromiumPath == string.Empty)
            throw new ArgumentException("Could not get chromium executable path. " +
                "Didn't manage to get the revision info automatically and the provided default chromium path doesn't exist");

        ct.ThrowIfCancellationRequested();

        Browser = await Puppeteer.LaunchAsync(
                         new LaunchOptions()
                         {
                             ExecutablePath = _chromiumPath,
                             Timeout = 300000,
                             Headless = true,
                             Args = new string[]
                             { "--no-zygote", "--no-sandbox", /*"--disable-dev-shm-usage",*/ 
                                 "--single-process", }
                         });


        ct.ThrowIfCancellationRequested();

        _webSocketEndpoint = Browser.WebSocketEndpoint;

        return Browser.IsConnected;
    }
    private async Task<string> DownloadChromiumIfNeeded(CancellationToken ct, IWebProxy? webProxy = null)
    {
        var browserFetcher = new BrowserFetcher();

        var revision = await browserFetcher.GetRevisionInfoAsync();

        var path = revision.ExecutablePath;

        if (revision.Downloaded && path != string.Empty)
        {
            return path;
        }

        if (webProxy is not null)
            browserFetcher.WebProxy = webProxy;
        
        var isAccessible = await browserFetcher
            .CanDownloadAsync(BrowserFetcher.DefaultChromiumRevision);

        if (!isAccessible)
        {
            var exception = webProxy == null
                ? new Exception("Cannot get access to an available chromium revision. Try using web proxy.")
                : new Exception("Cannot get access to an available chromium revision with the provided web proxy.");

            throw exception;
        }

        var info = await browserFetcher.DownloadAsync();

        if (!info.Downloaded)
        {
            throw new Exception("Didn't manage to download chromium");
        }

        return info.ExecutablePath;
    }
}


