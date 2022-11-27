namespace ScheduleUpdateService.Abstractions
{
    public interface IHashingService
    {
        public string GetHashSum<T>(T tObject);
    }

}
