using Microsoft.CodeAnalysis.CSharp.Syntax;
using Telegram.Bot.Types;
using TelegramBot.Commands;
using TelegramBot.Abstractions;
using TelegramBot.Models;
using TelegramBot.Services;

namespace TelegramBot.ChainOfConditions;

public class GroupInputFalseChainMember : AbstractMessageChainMember
{
    public override ICommand<ICommandArgs, Task<Message>>? Handle(ICommandArgs args)
    {
        bool condition = 
            args.User == null
            && args.OperationType == OperationType.Other
            && args.Update.Message != null;

        if (condition)
            return new InvalidGroupInputCommand(args); 
        else
            return base.Handle(args);
    }

}
