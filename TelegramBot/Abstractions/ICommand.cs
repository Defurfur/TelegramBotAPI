namespace TelegramBot.Abstractions
{
    public interface ICommand<TSource, TResult>
    {
        public TResult ExecuteAsync();
    }
}
