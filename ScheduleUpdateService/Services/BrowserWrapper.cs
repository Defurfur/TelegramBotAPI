using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using ScheduleUpdateService.Abstractions;
using System.Net;

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
        catch (TaskCanceledException tcEx)
        {
            _logger.LogWarning(tcEx, "[BrowserWrapper] Didn't manage to initialize broser, because " +
                "Cancellation Token has been requested");
            throw;
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "[BrowserWrapper] Didn't manage to initialize browser.");
            throw;
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
            _chromiumKiller.KillChromiumProcesses(_chromiumPath == string.Empty ? _defaultChromiumPath : _chromiumPath);
            IsInit = false;
        }


    }

    private async Task<bool> TryConnectToBrowserAsync(CancellationToken ct)
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
    private async Task<bool> TryLaunchBrowser(CancellationToken ct)
    {
        if (await LaunchBrowser(ct, _defaultChromiumPath))
        {
            _webSocketEndpoint = Browser!.WebSocketEndpoint;
            return Browser!.IsConnected;
        }

        _chromiumPath = await DownloadChromiumIfNeeded(ct);

        if (_chromiumPath == string.Empty)
            throw new ArgumentException("Could not get chromium executable path. " +
                "Didn't manage to get the revision info automatically and the provided default chromium path doesn't exist");

        ct.ThrowIfCancellationRequested();

        if (await LaunchBrowser(ct, _chromiumPath))
        {
            _webSocketEndpoint = Browser!.WebSocketEndpoint;
            return Browser!.IsConnected;
        }

        return false;
    }
    private async Task<bool> LaunchBrowser(CancellationToken ct, string path)
    {
        ct.ThrowIfCancellationRequested();

        try
        {
            Browser = await Puppeteer.LaunchAsync(
                          new LaunchOptions()
                          {
                              ExecutablePath = path,
                              Timeout = 300000,
                              Headless = true,
                              Args = new string[]
                              { "--no-zygote", "--no-sandbox", /*"--disable-dev-shm-usage",*/ 
                                     "--single-process", }
                          });
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "[{this}] Failed to gracefully launch browser instance using this path: {Path}",
                GetType().Name,
                path);
        }

        return Browser?.IsConnected ?? false;
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


