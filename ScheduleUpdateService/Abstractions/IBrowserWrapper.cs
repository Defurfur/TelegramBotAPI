using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdateService.Abstractions
{
    public interface IBrowserWrapper : IAsyncDisposable
    {
        public IBrowser? Browser { get;}
        bool IsInit { get;}
        bool IsBeingInitialized { get;}
        ValueTask InitAsync(CancellationToken ct);
    }
}
