using Telegram.Bot.Types;
using TelegramBotService.Commands;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;

namespace TelegramBotService.ChainOfConditions;

public class DownloadScheduleChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User != null
            && args.OperationType == OperationType.DownloadScheduleRequest
            && args.ScheduleLoader != null;

        if (condition)
            return new DownloadScheduleCommand(args);
        else
            return base.Handle(args);
    }

}
