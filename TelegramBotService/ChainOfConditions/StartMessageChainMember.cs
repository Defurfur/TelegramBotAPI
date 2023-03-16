using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;

namespace TelegramBotService.ChainOfConditions;

public class StartMessageChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User == null
            && args.OperationType == OperationType.StartCommand
            && args.Update.Message != null;

        if (condition)
            return new ShowStartMessageCommand(args);
        else
            return base.Handle(args);
    }

}




