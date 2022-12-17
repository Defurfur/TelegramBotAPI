using Telegram.Bot.Types;
using TelegramBotService.ChainOfConditions;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services;

public static class ChainStorage
{
    private static List<IChainMember<ICommandArgs, Task<Message>>> _chainMembers = new()
    {
        new SwithWeekCallbackChainMember(),
        new CreateSettingsCallbackChainMember(),
        new EnableSubscriptionCallbackChainMember(),
        new DisableSubscriptionCallbackChainMember(),
        new ProcessCallbackChainMember(),
        new TryChangeGroupChainMember(),
        new ChangeSubscriptionSettingsChainMember(),
        new StartMessageChainMember(),
        new GroupInputFalseChainMember(),
        new GroupInputTrueChainMember(),
        new DownloadScheduleChainMember(),
        new ChangeGroupButtonChainMember(),
        new ChangeSubscriptionSettingsChainMember(),
    };

    public static List<IChainMember<ICommandArgs, Task<Message>>> ChainMembers { get => _chainMembers; }
}
