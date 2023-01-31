using Microsoft.EntityFrameworkCore.Metadata;
using ScheduleUpdateService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Xunit;
using Humanizer;
using XSystem.Security.Cryptography;
using System.Diagnostics;
using ScheduleUpdateService.Abstractions;

namespace UnitTests.ScheduledUpdateServiceTests
{
    public class BrowserWrapperTests
    {
        private IChromiumKiller? _chromiumKiller;

        private readonly Mock<ILogger<BrowserWrapper>> _loggerWrapperMock = new();
        private readonly Mock<ILogger<ChromiumKiller>> _loggerChromiumMock = new();
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _configMock = new();
        public BrowserWrapperTests()
        {
        }


        [Fact]
        public async Task TryLaunchBrowser_Launches_Browser_And_Assigns_WSField()
        {

            //ARRANGE
            if (_chromiumKiller is null)
                _chromiumKiller = new ChromiumKiller(_loggerChromiumMock.Object);

            await using var browserWrapper = new BrowserWrapper(_loggerWrapperMock.Object, _chromiumKiller, _configMock.Object);

            string path = """C:\inetpub\wwwroot\REATelegramAPI_dev\.local-chromium\Win64-970485\chrome-win\chrome.exe""";

            _chromiumKiller.KillChromiumProcesses(path, 3000);

            var method = browserWrapper
                .GetType()
                .GetMethod("TryLaunchBrowser", BindingFlags.NonPublic | BindingFlags.Instance);

            //ACT
            var result = await method.InvokeAsync(browserWrapper);

            var webSocketEndpointField = browserWrapper
                .GetType()
                .GetField("_webSocketEndpoint", BindingFlags.Instance | BindingFlags.NonPublic);

            string wsFieldValue = webSocketEndpointField?.GetValue(browserWrapper)?.ToString() ?? string.Empty;

            //ASSERT
            Assert.True((bool)result);
            Assert.True(wsFieldValue.Contains("ws"));

        }
        [Fact]
        public async Task TryConnectToBrowserAsync_Connects_ToExisting_Chromium()
        {

            //ARRANGE
            if(_chromiumKiller is null)
                _chromiumKiller = new ChromiumKiller( _loggerChromiumMock.Object);

            await using var browserWrapper = new BrowserWrapper(_loggerWrapperMock.Object, _chromiumKiller, _configMock.Object);

            string path = """C:\inetpub\wwwroot\REATelegramAPI_dev\.local-chromium\Win64-970485\chrome-win\chrome.exe""";

            _chromiumKiller.KillChromiumProcesses(path, 3000);

            var tryLaunchBrowser = browserWrapper
                .GetType()
                .GetMethod("TryLaunchBrowser", BindingFlags.NonPublic | BindingFlags.Instance);

            var tryConnectToBrowserAsync = browserWrapper
                .GetType()
                .GetMethod("TryConnectToBrowserAsync", BindingFlags.NonPublic | BindingFlags.Instance);

            var initialChromium = await tryLaunchBrowser?.InvokeAsync(browserWrapper) ?? null;

            var wsEndpointField = browserWrapper
                .GetType()
                .GetField("_webSocketEndpoint", BindingFlags.Instance | BindingFlags.NonPublic);

            string wsFieldValue = wsEndpointField?
                .GetValue(browserWrapper)?
                .ToString() ?? string.Empty;

            if (wsFieldValue == string.Empty)
            {
                Assert.Fail("_webSocketEndpoint value occurred to be empty");
                return;
            }

            //ACT

            var initialChromiums = Process
                .GetProcessesByName("chrome")
                .Where(x => x.MainModule?.FileName == path)
                .ToList();

            var initialAmount = initialChromiums.Count();

            var connectAttempt = await tryConnectToBrowserAsync.InvokeAsync(browserWrapper);

            var afterAttemptChromiums = Process
                .GetProcessesByName("chrome")
                .Where(x => x.MainModule?.FileName == path)
                .ToList();
            var afterAttemptAmount = afterAttemptChromiums.Count();

            //ASSERT
            Assert.True((bool)connectAttempt);
            Assert.Equal(initialAmount, afterAttemptAmount);

        }


    }
}
