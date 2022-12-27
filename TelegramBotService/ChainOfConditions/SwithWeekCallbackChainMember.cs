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

// review - error - typo
public class SwithWeekCallbackChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User != null
            && args.Callback != null
            && args.Callback.Data != null
            && args.CallbackMessageUpdater != null
            && args.OperationType == OperationType.SwitchWeekCallback
            && args.ScheduleLoader != null;

        if (condition)
            return new SwitchWeekCallbackCommand(args);
        else
            return base.Handle(args);
    }

}