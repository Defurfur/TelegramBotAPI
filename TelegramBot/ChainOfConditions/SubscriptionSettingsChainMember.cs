using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Abstractions;
using TelegramBot.Models;

namespace TelegramBot.ChainOfConditions;

public class SubscriptionSettingsChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User != null &&
            args.OperationType == OperationType.ChangeSubscriptionSettingsRequest;

        if (condition)
            return new ChangeSubscriptionSettingsCommand(args);
        else
            return base.Handle(args);
    }

}
