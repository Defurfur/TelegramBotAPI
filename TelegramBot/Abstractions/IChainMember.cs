namespace TelegramBot.Abstractions
{
    public interface IChainMember<TSource, TResult>
    {
        IChainMember<TSource, TResult> SetNext(IChainMember<TSource, TResult> chainMember);
        ICommand<TSource, TResult>? Handle(ICommandArgs args);
    }
}
