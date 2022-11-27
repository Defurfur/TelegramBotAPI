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
        public Browser? Browser { get; set; }
        bool IsInit { get; set; }
        Task Init();
    }
}
