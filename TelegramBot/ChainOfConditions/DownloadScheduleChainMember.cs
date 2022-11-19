using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Abstractions;
using TelegramBot.Models;

namespace TelegramBot.ChainOfConditions;

public class DownloadScheduleChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition =
            args.User != null &&
            args.OperationType == OperationType.DownloadScheduleRequest;

        if (condition)
            return new DownloadScheduleCommand(args);
        else
            return base.Handle(args);
    }

}
