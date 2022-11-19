using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Abstractions;
using TelegramBot.Commands;
using TelegramBot.Models;

namespace TelegramBot.ChainOfConditions;

public class ProcessCallbackChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User != null
            && args.Callback != null
            && args.Callback.Data != null
            && args.CallbackMessageUpdater != null
            && args.UserUpdater != null;

        if (condition)
            return new ProcessCallbackCommand(args);
        else
            return base.Handle(args);
    }

}