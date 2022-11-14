using Jint.Parser.Ast;
using Telegram.Bot.Types;
using TelegramBot.Chain_of_commands;
using TelegramBot.Interfaces;

namespace TelegramBot.Services
{
    public static class ChainStorage
    {
        private static List<IChainMember<ICommandArgs, Task<Message>>> _chainMembers = new()
        {
            new StartMessageChainMember(),
            new GroupInputFalseChainMember(),
            new GroupInputTrueChainMember(),
        };

        public static List<IChainMember<ICommandArgs, Task<Message>>> ChainMembers { get => _chainMembers; }
    }
}
