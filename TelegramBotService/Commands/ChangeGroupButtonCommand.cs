using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using TelegramBotService.Services;

namespace TelegramBotService.Commands;

public class ChangeGroupButtonCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly IUserUpdater _updater;
    private readonly ReaSchedule.Models.User _user;
    private readonly Message _message;
    public ChangeGroupButtonCommand(ICommandArgs args)
    {
        _user = args.User!;
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
        _updater = args.UserUpdater!;
    }

    public async Task<Message> ExecuteAsync()
    {
        var groupName = await _updater.GetUserGroupname(_user);

        return await _sender.SendChangeGroupInfo(_message, groupName);
    }
}
