using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleWorker
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
                    new LaunchOptions { Headless = true, Args = new string[] { "--no-zygote", "--single-process" } });
                IsInit = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (IsInit)
            {
                await Browser!.CloseAsync();
                await Browser!.DisposeAsync();
                IsInit = false;
            }
        }
    }
}
