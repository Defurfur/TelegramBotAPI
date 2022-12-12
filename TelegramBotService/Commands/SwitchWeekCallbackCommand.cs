using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using TelegramBotService.Services;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Commands;

public class SwitchWeekCallbackCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly CallbackQuery _callback;
    private readonly ICallbackMessageUpdater _messageUpdater;
    private readonly IUserUpdater _userUpdater;
    private readonly User _user;
    private readonly IScheduleLoader _scheduleLoader;
    public SwitchWeekCallbackCommand(ICommandArgs args)
    {
        _user = args.User!;
        _callback = args.Callback!;
        _messageUpdater = args.CallbackMessageUpdater!;
        _userUpdater = args.UserUpdater!;
        _scheduleLoader = args.ScheduleLoader!;
    }

    public async Task<Message> ExecuteAsync()
    {
       var exists = int.TryParse(_callback.Data!.Split(" ")[1], out var weekNumber);

        var formattedWeek = await _scheduleLoader.DownloadFormattedScheduleAsync(_user, weekNumber - 1);

        var result = _callback.Data switch
        {
            "ScheduleSwitchers: 1" => _messageUpdater.UpdateWithCustomTextAndKeyboard(
                _callback,
                formattedWeek,
                CustomKeyboardStorage.WeekScheduleSwitchersSetOnOne),

            "ScheduleSwitchers: 2" => _messageUpdater.UpdateWithCustomTextAndKeyboard(
                _callback,
                formattedWeek,
                CustomKeyboardStorage.WeekScheduleSwitchersSetOnTwo),

                _ => _messageUpdater.UpdateWithErrorMessage(_callback)
        };
        //TODO: Выделить сегодняшний день каким-нибудь смайликом

        return await result;
    }
}
