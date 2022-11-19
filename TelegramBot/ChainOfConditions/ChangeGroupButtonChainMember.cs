using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Abstractions;
using TelegramBot.Models;

namespace TelegramBot.ChainOfConditions;

public class ChangeGroupButtonChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User != null &&
            args.OperationType == OperationType.ChangeGroupButtonPressed;

        if (condition)
            return new ChangeGroupButtonCommand(args);
        else
            return base.Handle(args);
    }

}
