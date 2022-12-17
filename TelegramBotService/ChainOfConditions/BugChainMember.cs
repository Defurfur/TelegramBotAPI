using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;

namespace TelegramBotService.ChainOfConditions;

public class BugChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.OperationType == OperationType.BugCommand
            && args.Update.Message != null
            && args.ContextUpdateService != null;

        if (condition)
            return new BugCommand(args);
        else
            return base.Handle(args);
    }

}




