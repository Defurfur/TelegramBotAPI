using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

namespace TelegramBotService.ChainOfConditions;

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
