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
    private readonly SubscriptionSettings? _settings;
 
    public ProcessCallbackCommand(ICommandArgs args)
    {
        _user = args.User!;
        _settings = args.User!.SubscriptionSettings;
        _callback = args.Callback!;
        _messageUpdater = args.CallbackMessageUpdater!;
        _userUpdater = args.UserUpdater!;
        _settings = args.User!.SubscriptionSettings;
    }

    public async Task<Message> ExecuteAsync()
    {
        await _userUpdater.ProcessCallbackAndSaveChanges(_settings!, _callback.Data!);

        if(_settings.UpdateSchedule == UpdateSchedule.EveryWeek
            && _callback.Data.Contains("TimeOfDay"))
        {
            return await _messageUpdater.UpdateWithSuccessMessage(_settings!, _callback);
        }

        if (_settings.UpdateSchedule == UpdateSchedule.EveryDay 
            && _callback.Data.Contains("IncludeToday"))
        {
            return await _messageUpdater.UpdateWithSuccessMessage(_settings!, _callback);

        }

        var kvp= DictionaryStorage
            .MessageUpdaterAndTasksDict
            .FirstOrDefault(x => _callback.Data!.Contains(x.Key));

        var task = kvp.Value;
        var result = task != null 
            ? kvp.Value.Invoke(_messageUpdater, _callback) 
            : _messageUpdater.UpdateWithErrorMessage(_callback);


        return await result;
    }
}
