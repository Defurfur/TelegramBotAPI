using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Services;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Commands;

public class ChangeSubscriptionSettingsCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly Message _message;
    private readonly User _user;
    private readonly SubscriptionSettings? _settings;
    public ChangeSubscriptionSettingsCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
        _user = args.User!;
        _settings = args.User!.SubscriptionSettings;
    }

    public async Task<Message> ExecuteAsync()
    {
        if(_settings is null)
            return await _sender.SendDefaultSubscriptionSettings(_message);

        return await _sender.SendSubscriptionSettings(_settings, _message, _settings.SubscriptionEnabled);
    }
}
