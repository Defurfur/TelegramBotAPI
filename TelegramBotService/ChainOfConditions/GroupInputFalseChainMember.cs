using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

namespace TelegramBotService.ChainOfConditions;

public class GroupInputFalseChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition = 
            args.User == null
            && args.OperationType == OperationType.Other
            && args.Update.Message != null;

        if (condition)
            return new InvalidGroupInputCommand(args); 
        else
            return base.Handle(args);
    }

}
