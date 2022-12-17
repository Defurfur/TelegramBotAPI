using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

namespace TelegramBotService.ChainOfConditions;

public class ChangeSubscriptionSettingsChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.Update.Message != null
            && args.User != null 
            && args.MessageSender != null 
            && args.OperationType == OperationType.ChangeSubscriptionSettingsRequest;

        if (condition)
            return new ChangeSubscriptionSettingsCommand(args);
        else
            return base.Handle(args);
    }

}
