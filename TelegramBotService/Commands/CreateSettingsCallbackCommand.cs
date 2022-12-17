using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Commands;

public class CreateSettingsCallbackCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly CallbackQuery _callback;
    private readonly ICallbackMessageUpdater _messageUpdater;
    private readonly IUserUpdater _userUpdater;
    private readonly User _user;
    public CreateSettingsCallbackCommand(ICommandArgs args)
    {
        _user = args.User!;
        _callback = args.Callback!;
        _messageUpdater = args.CallbackMessageUpdater!;
        _userUpdater = args.UserUpdater!;
    }

    public async Task<Message> ExecuteAsync()
    {
        var settings = await _userUpdater.CreateSubscriptionSettings(_user);

        await _userUpdater.ProcessCallbackAndSaveChanges(settings, _callback.Data!);

        var result = _messageUpdater.UpdateWithScheduleFrequencyOptionsKeyboard(_callback);

        return await result;
    }
}
