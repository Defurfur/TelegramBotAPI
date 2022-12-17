using ReaSchedule.Models;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Commands;

public class BugCommand : ICommand<ICommandArgs, Task<Message>>
{
    private readonly IMessageSender _sender;
    private readonly Message _message;
    private readonly IContextUpdateService _updateService;
    private readonly User? _user;
    public BugCommand(ICommandArgs args)
    {
        _user = args.User;
        _message = args.Update.Message!;
        _sender = args.MessageSender!;
        _updateService = args.ContextUpdateService!;
    }

    public async Task<Message> ExecuteAsync()
    {
        var text = _message.Text!.Remove(0, 4);

        if (text.Length <= 8)
            return await _sender.SendMessageWithSomeText(
                _message, 
                "Сообщение не сохранено\\: произошла какая\\-то ошибка\\.\r\n" +
                "Убедитесь, что длина текста больше 8 символов\\.");


        var task = _user == null
            ?  _updateService.AddBug(_message.Chat.Id, text)
            :  _updateService.AddBug(_message.Chat.Id, text, _user.Id);

        await task;

        return await _sender.SendMessageWithSomeText(_message, "\\*Сообщение сохранено\\!\\*");
    }
}




