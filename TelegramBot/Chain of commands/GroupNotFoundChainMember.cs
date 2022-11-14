using Microsoft.CodeAnalysis.CSharp.Syntax;
using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Interfaces;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.Chain_of_commands;

public class GroupNotFoundChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition = args.User == null
            && args.OperationType == OperationType.IsGroupInput
            && args.Update.Message != null;

        if (condition)
            return new InvalidGroupInputCommand(args); 
        else
            return base.Handle(args);
    }

}
