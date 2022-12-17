using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Commands;
using TelegramBotService.Models;

namespace TelegramBotService.ChainOfConditions;

public class DisableSubscriptionCallbackChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User != null
            && args.User.SubscriptionSettings != null
            && args.Callback != null
            && args.Callback.Data == "Disable Subscription"
            && args.CallbackMessageUpdater != null
            && args.UserUpdater != null;

        if (condition)
            return new DisableSubscriptionCommand(args);
        else
            return base.Handle(args);
    }

}