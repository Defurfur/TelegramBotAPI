using Humanizer;
using Microsoft.Extensions.Logging;
using ScheduleUpdateService.Abstractions;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace ScheduleUpdateService.Services;

public class ChromiumKiller : IChromiumKiller
{
    private readonly ILogger<ChromiumKiller> _logger;
    public ChromiumKiller(ILogger<ChromiumKiller> logger)
    {
        _logger = logger;
    }
    public void KillChromiumProcesses(string path = "", int timeout = 5000)
    {
        List<Process> processes = new();

        try
        {
            _logger.LogWarning("[ChromiumKiller] Trying to kill processes associated with path: {Path}", path);

            processes =
                path == ""
                ? GetProcesses()
                : GetProcesses(path);

        }
        catch (Win32Exception win32ex)
        {
            if(_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogWarning(
                    win32ex,
                    "[{this}] Win32Exception has been thrown during while an attempt to KillChromiumProccesses with path: {path}",
                    GetType().Name,
                    path);
            };
        }


        _logger.LogInformation("[{this}] Starting process of killing chromiums. " +
            "{chromiumCount} chromiums found.", GetType().Name, processes.Count);

        if (!processes.Any())
        {
            _logger.LogDebug("[{this}] Found zero processes", GetType().Name);
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
            catch(Win32Exception) {}
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{this}] {exName} occured when trying to kill chrome process",
                    GetType().Name,
                    ex.GetType().Name);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation("[{this}] {processNumber} process killed within {elapsedTime}",
                    GetType().Name,
                    i + 1,
                    stopwatch.Elapsed.Humanize(2));

                stopwatch.Reset();
            }
        }
    }

    private List<Process> GetProcesses(string path)
    {
        var chromiums = GetProcesses();
        var processes = new List<Process>();
        int failCounter = 0;

        foreach(var process in chromiums)
        {
            try
            {
                if (process.MainModule?.FileName == path)
                    processes.Add(process);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "[ChromiumKiller] {ExceptionName} has been thrown when trying " +
                    "to get MainModule of process with Id {ProcessID}",
                    ex.GetType().Name,
                    process);

                failCounter++;
            }
        }

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("[{this}] Accessed {RecievedProcesses} out of " +
                "{TotalProcesses} chromium processes with the given path: {Path}.\r\n" +
                " {FailedProcesses} processes couldn't be accessed",
                GetType().Name,
                chromiums.Count - failCounter,
                chromiums.Count,
                path,
                failCounter);
        }

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