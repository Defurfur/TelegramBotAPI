using Telegram.Bot.Types;
using TelegramBot.ChainOfConditions;
using TelegramBot.Abstractions;

namespace TelegramBot.Services;

public static class ChainStorage
{
    private static List<IChainMember<ICommandArgs, Task<Message>>> _chainMembers = new()
    {
        new ProcessCallbackChainMember(),
        new TryChangeGroupChainMember(),
        new SubscriptionSettingsChainMember(),
        new StartMessageChainMember(),
        new GroupInputFalseChainMember(),
        new GroupInputTrueChainMember(),
        new DownloadScheduleChainMember(),
        new ChangeGroupButtonChainMember(),
        new SubscriptionSettingsChainMember(),
    };

    public static List<IChainMember<ICommandArgs, Task<Message>>> ChainMembers { get => _chainMembers; }
}
