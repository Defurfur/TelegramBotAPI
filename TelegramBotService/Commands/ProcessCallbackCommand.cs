using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Commands;

public class ProcessCallbackCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly CallbackQuery _callback;
    private readonly ICallbackMessageUpdater _messageUpdater;
    private readonly IUserUpdater _userUpdater;
    private readonly User _user;
    public ProcessCallbackCommand(ICommandArgs args)
    {
        _user = args.User!;
        _callback = args.Callback!;
        _messageUpdater = args.CallbackMessageUpdater!;
        _userUpdater = args.UserUpdater!;
    }

    public async Task<Message> ExecuteAsync()
    {
        var result = _callback.Data switch
        {
            "Enable Subscription" => _messageUpdater.UpdateWithScheduleFrequencyOptionsKeyboard(_callback),
            "Show Schedule: every day" => _messageUpdater.UpdateWithDayNumberOptionsKeyboard(_callback),
            "Show Schedule: every week" => _messageUpdater.UpdateWithWeeklyScheduleOptionsKeyboard(_callback),
            "Disable Subscription" => _messageUpdater.UpdateWithSubscriptionDisabled(_callback),
            "Change subscription settings" => _messageUpdater.UpdateWithScheduleFrequencyOptionsKeyboard(_callback),
            _ => _messageUpdater.UpdateWithSuccessMessage(_callback)
        };

        await _userUpdater.ProcessCallbackAndSaveChanges(_user, _callback.Data!);

        return await result;
    }
}
