using Telegram.Bot.Types;
using TelegramBot.Chain_of_commands;
using TelegramBot.Commands;
using TelegramBot.Interfaces;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Chain_of_commands;

public class StartMessageChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User == null &&
            args.OperationType == OperationType.IsStartCommand &&
            args.Update.Message != null;

        if (condition)
            return new ShowStartMessageCommand(args);
        else
            return base.Handle(args);
    }

}




