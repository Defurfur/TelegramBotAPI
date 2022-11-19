using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBot.Abstractions;
using User = ReaSchedule.Models.User;

namespace TelegramBot.Commands;

public class DownloadScheduleCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _reciever;
    private readonly Message _message;
    private readonly User _user;
    public DownloadScheduleCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _reciever = args.MessageSender!;
        _user = args.User!;
    }

    public async Task<Message> ExecuteAsync()
    {

        return await _reciever.ShowStartMessage(_message);
    }
}




