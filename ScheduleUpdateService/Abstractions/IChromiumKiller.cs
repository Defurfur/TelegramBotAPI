namespace ScheduleUpdateService.Abstractions
{
    public interface IChromiumKiller
    {
        void KillChromiumProcesses(string path = "", int timeout = 5000);
    }
}