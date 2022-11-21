using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

namespace TelegramBotService.ChainOfConditions;

public class TryChangeGroupChainMember: AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition = 
            args.User != null
            && args.OperationType == OperationType.GroupChangeCommand
            && args.Update.Message != null;

        if (condition)
            return new TryChangeGroupCommand(args); 
        else
            return base.Handle(args);
    }

}
