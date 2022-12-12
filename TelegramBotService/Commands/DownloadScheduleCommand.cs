using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Commands;

public class DownloadScheduleCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly IScheduleLoader _loader;
    private readonly Message _message;
    private readonly User _user;
    public DownloadScheduleCommand(ICommandArgs args)
    {
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
        _user = args.User!;
        _loader = args.ScheduleLoader!;
    }

    public async Task<Message> ExecuteAsync()
    {
        var formattedSchedule = await _loader.DownloadFormattedScheduleAsync(_user, 0);

        return await _sender.DownloadScheduleMessageWithKeyboard(_message, formattedSchedule);
    }
}




