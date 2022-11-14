namespace TelegramBot.Interfaces
{
    public interface ICommand<TSource, TResult>
    {
        public TResult ExecuteAsync();
    }
}
