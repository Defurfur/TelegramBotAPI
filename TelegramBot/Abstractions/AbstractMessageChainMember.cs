using Telegram.Bot.Types;

namespace TelegramBot.Abstractions;

public abstract class AbstractMessageChainMember : IChainMember<ICommandArgs, Task<Message>>
{
    private IChainMember<ICommandArgs, Task<Message>>? _nextChainMember;
    public IChainMember<ICommandArgs, Task<Message>> SetNext(IChainMember<ICommandArgs, Task<Message>> chainMember)
    {
        _nextChainMember = chainMember;
        return chainMember;
    }
    public virtual ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        if (_nextChainMember is not null)
            return _nextChainMember.Handle(args);
        else
            return null;
    }

}
