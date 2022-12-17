using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Commands;

public class DisableSubscriptionCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly CallbackQuery _callback;
    private readonly ICallbackMessageUpdater _messageUpdater;
    private readonly IUserUpdater _userUpdater;
    private readonly SubscriptionSettings? _settings;

    public DisableSubscriptionCommand(ICommandArgs args)
    {
        _settings = args.User!.SubscriptionSettings;
        _callback = args.Callback!;
        _messageUpdater = args.CallbackMessageUpdater!;
        _userUpdater = args.UserUpdater!;
        _settings = args.User!.SubscriptionSettings;
    }

    public async Task<Message> ExecuteAsync()
    {

        await _userUpdater.ProcessCallbackAndSaveChanges(_settings!, _callback.Data!);

        var result = _messageUpdater.UpdateWithSubscriptionKeyboard(
             _callback,
             _settings!,
             _settings!.SubscriptionEnabled);

        return await result;
    }
}
