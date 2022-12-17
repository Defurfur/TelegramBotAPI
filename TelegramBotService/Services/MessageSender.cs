using ReaSchedule.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService.Abstractions;
using Message = Telegram.Bot.Types.Message;
using Humanizer;

namespace TelegramBotService.Services;

public class MessageSender : IMessageSender
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserSettingsFormatter _settingsFormatter;

    public MessageSender(ITelegramBotClient bot,
        IUserSettingsFormatter settingsFormatter)
    {
        _bot = bot;
        _settingsFormatter = settingsFormatter;
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
            text: "Группа найдена! Теперь вы можете пользоваться ботом!",
            replyMarkup: CustomKeyboardStorage.DefaultReplyKeyboard
            );
    } 
    public async Task<Message> InvalidGroupInputMessage(Message message)
    {
        return await _bot.SendTextMessageAsync(
            chatId: message!.Chat.Id,
            text: "Неправильно введена группа." +
                " Удалите пробелы и убедитесь, что вы правильно написали номер. Если вы уверены," +
                "что написали номер группы правильно, но все еще получаете это сообщение" +
                " - скопируйте название группы из личного кабинета или сайта с расписанием");
    } 
    public async Task<Message> GroupNotFoundMessage(Message message)
    {

        return await _bot.SendTextMessageAsync(
            chatId: message!.Chat.Id,
            text: "Группа не найдена ни в базе, " +
            "ни в расписании. Убедитесь, что правильно ввели название группы");
    } 
    public async Task<Message> SendChangeGroupInfo(Message message)
    {

        return await _bot.SendTextMessageAsync(
            chatId: message!.Chat.Id,
            text: "Чтобы поменять группу, введите /change <название группы>" +
            "\r\nНапример, /change 15.02Д-ММ2/19б"
            ); 
    }
    public async Task<Message> ChangeGroupSuccess(Message message)
    {

        return await _bot.SendTextMessageAsync(
            chatId : message!.Chat.Id,
            text : "Группа успешно изменена!"
            );
    }
    public async Task<Message> SendDefaultSubscriptionSettings(Message message)
    {

        return await _bot.SendTextMessageAsync(
            chatId : message!.Chat.Id,
            text : "Подписка позволяет получать расписание автоматически в указанное вами время."
            + "Например, вы можете получать недельное расписание каждую неделю в указанный вами день, либо" 
            + "получать расписание на день в день занятия.",
            replyMarkup: CustomKeyboardStorage.NoSubscriptionKeyboard
            );
    }
    public async Task<Message> SendSubscriptionSettings(
        SubscriptionSettings settings,
        Message message,
        bool subscriptionEnabled)
    {
        var keyboard = subscriptionEnabled == true
            ? CustomKeyboardStorage.SubscriptionEnabledKeyboard
            : CustomKeyboardStorage.SubscriptionDisabledKeyboard;

        string formattedSettings = _settingsFormatter.Format(settings);

        return await _bot.SendTextMessageAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
            chatId: message!.Chat.Id,
            text: "*Ваши текущие настройки:* \r\n\r\n" + formattedSettings,
            replyMarkup: keyboard
            );
    }

    public async Task<Message> SendMessageWithSomeText(Message message, string text)
    {
        return await _bot.SendTextMessageAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
            chatId: message!.Chat.Id,
            text: text
            );
    }    
    public async Task<Message> SendMessageWithSomeText(long chatId, string text)
    {
        return await _bot.SendTextMessageAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
            chatId: chatId,
            text: text
            );
    }

    public async Task<Message> SendGroupSearchInProcess(Message message)
    {
        return await _bot.SendTextMessageAsync(
            chatId: message!.Chat.Id,
            text: "Введенной вами группы пока нет в базе данных." +
            " Если вы ввели название группы правильно, то мы добавим расписание" +
            "в базу и отправим вам сообщение об успешном добавлении группы, после чего вы сможете пользоваться ботом " +
            "\r\nПроцедура может занять несколько минут."
            );
    }

    public async Task<Message> SomethingWentWrongMessage(Message message)
    {
        return await _bot.SendTextMessageAsync(
            chatId: message!.Chat.Id,
            text: "Возникла какая-то ошибка... Попробуйте повторить операцию"
            );


    }

    public async Task<Message> DownloadScheduleMessageWithKeyboard(Message message, string text)
    {
        return await _bot.SendTextMessageAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
            chatId: message!.Chat.Id,
            text: text,
            replyMarkup: CustomKeyboardStorage.WeekScheduleSwitchersSetOnOne
            );
    }
  

}
