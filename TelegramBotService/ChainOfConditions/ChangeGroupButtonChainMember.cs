using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

namespace TelegramBotService.ChainOfConditions;

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
