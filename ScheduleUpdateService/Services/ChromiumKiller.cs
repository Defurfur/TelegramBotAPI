using Humanizer;
using Microsoft.Extensions.Logging;
using ScheduleUpdateService.Abstractions;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace ScheduleUpdateService.Services;

public enum ConfigSource
{
    All,
    Debug,
    Release
}

public class ChromiumKiller : IChromiumKiller
{
    private readonly ILogger<ChromiumKiller> _logger;
    public ChromiumKiller(ILogger<ChromiumKiller> logger)
    {
        _logger = logger;
    }
    public void KillChromiumProcesses(string path = default, int timeout = 5000)
    {
        List<Process> processes = new();

        try
        {
            processes =
                path == default
                ? GetProcesses()
                : GetProcesses(path);

        }
        catch (Win32Exception win32ex)
        {
            if(_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    win32ex,
                    "Win32Exception has been thrown during while an attempt to KillChromiumProccesses with path = {path}",
                    path);
            };
        }


        _logger.LogDebug("[ChromiumKiller] Starting process of killing chromiums. " +
            "{chromiumCount} chromiums found.", processes.Count);

        if (!processes.Any())
        {
            _logger.LogDebug("[ChromiumKiller] Found zero processes");
            return;
        }

        KillProcesses(processes, timeout);

    }

    private void KillProcesses(List<Process> processes, int timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        stopwatch.Reset();

        for (var i = 0; i < processes.Count; i++)
        {
            stopwatch.Start();

            try
            {
                if (!processes[i].WaitForExit(timeout))
                {
                    if (!processes[i].HasExited)
                    {
                        processes[i].Kill();
                        Thread.Sleep(10);
                    }
                }
            }
            catch(Win32Exception winEx)
            {
                continue;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[ChromiumKiller] {exName} occured when trying to kill chrome process",
                    ex.GetType().Name);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation("{processNumber} process killed within {elapsedTime}",
                    i + 1,
                    stopwatch.Elapsed.Humanize(2));

                stopwatch.Reset();
            }
        }
    }

    private List<Process> GetProcesses(string path)
    {
        var processes = Process
               .GetProcessesByName("chrome")
               .Where(x => x.MainModule?.FileName == path)
               .ToList();


        return processes;
    }
    private List<Process> GetProcesses()
    {
        var processes = Process
               .GetProcessesByName("chrome")
               .ToList();


        return processes;
    }
}