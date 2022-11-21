namespace TelegramBotService.Abstractions
{
    public interface ICommand<TSource, TResult>
    {
        public TResult ExecuteAsync();
    }
}
