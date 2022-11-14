using Jint.Native;
using Jint.Parser.Ast;
using Microsoft.AspNetCore.Components.Forms;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Interfaces;
using XAct.Messages;
using Message = Telegram.Bot.Types.Message;
using User = ReaSchedule.Models.User;

namespace TelegramBot.Services;

public enum MessageTextType
{
    NotYetProcessed,
    StartCommand,
    GroupInput,
    Other
}

public enum GroupHasBeenFound
{
    InDatabase,
    InSchedule,
    False,
}
public class MessageSender : IMessageSender
{
    private readonly ITelegramBotClient _bot;

    public MessageSender(
        ITelegramBotClient bot
        )
    {
        _bot = bot;
    }

    public async Task<Message> ShowStartMessage(Message message)
    {
        return await _bot.SendTextMessageAsync(
            chatId: message!.Chat.Id,
            text: "Приветствую! Вы запустили бот расписания университета РЭУ им. Плеханова." +
            " Чтобы начать пользоваться ботом, напишите название группы, для которой вы хотите посмотреть расписание");
    }
    public async Task<Message> GroupFoundMessage(Message message)
    {
        return await _bot.SendTextMessageAsync(
            chatId: message!.Chat.Id,
            text: "Группа найдена! Теперь вы можете пользоваться ботом!");
    } 
    public async Task<Message> InvalidGroupInputMessage(Message message)
    {

        return await _bot.SendTextMessageAsync(message!.Chat.Id, "Неправильно введена группа." +
                " Удалите пробелы и убедитесь, что вы правильно написали номер. Если вы уверены," +
                "что написали номер группы правильно, но все еще получаете это сообщение" +
                " - скопируйте его из личного кабинета или сайта с расписанием");
    } 
    public async Task<Message> GroupNotFoundMessage(Message message)
    {

        return await _bot.SendTextMessageAsync(message!.Chat.Id, "Группа не найдена ни в базе, " +
            "ни в расписании. Убедитесь, что правильно ввели название группы");
    }

}
