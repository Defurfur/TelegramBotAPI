namespace ScheduleUpdateService.Abstractions
{
    public interface IChromiumKiller
    {
        void KillChromiumProcesses(string path = null, int timeout = 5000);
    }
}