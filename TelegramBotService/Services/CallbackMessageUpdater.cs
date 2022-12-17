using ReaSchedule.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services;

public class CallbackMessageUpdater : ICallbackMessageUpdater
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserSettingsFormatter _settingsFormatter;
    public CallbackMessageUpdater(
        ITelegramBotClient bot,
        IUserSettingsFormatter settingsFormatter)
    {
        _bot = bot;
        _settingsFormatter = settingsFormatter;
    }

    public async Task<Message> UpdateWithScheduleFrequencyOptionsKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Выберите интервал, по которому будет присылаться расписание:",
            replyMarkup: CustomKeyboardStorage.ScheduleFrequencyOptionsKeyboard
            );
    }
    public async Task<Message> UpdateWithDayNumberOptionsKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Выберите количество дней с расписанием, которые вы хотите получать",
            replyMarkup: CustomKeyboardStorage.DayNumberOptionsKeyboard
            );
    }

    public async Task<Message> UpdateWithSubscriptionKeyboard(
        CallbackQuery callback,
        SubscriptionSettings settings,
        bool subscriptionEnabled)
    {
        var keyboard = subscriptionEnabled == true 
            ? CustomKeyboardStorage.SubscriptionEnabledKeyboard 
            : CustomKeyboardStorage.SubscriptionDisabledKeyboard;

        var formattedText = _settingsFormatter.Format(settings); 

        return await _bot.EditMessageTextAsync(
           parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
           chatId: callback.Message.Chat.Id,
           messageId: callback.Message.MessageId,
           text: "*Ваши текущие настройки* \r\n\r\n" + formattedText,
           replyMarkup: keyboard
           );
    }
 
 
    public async Task<Message> UpdateWithWeekToSendKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Расписание какой недели вам присылать. \r\n" +
            "Например, если вы получаете расписание в пятницу и выбрали первый вариант " +
            "- вы получите расписание недели, содержащее эту пятницу.\r\n" +
            "Если вы выбрали второй вариант - вы получите расписание следующей недели",
            replyMarkup: CustomKeyboardStorage.WeeksToSendKeyboard
            );
    }
    public async Task<Message> UpdateWithTimeOfDayKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Выберите, в какое время суток присылать расписание",
            replyMarkup: CustomKeyboardStorage.TimeOfDayKeyboard
            );
    }
    public async Task<Message> UpdateWithIncludeTodayKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Выберите должно ли расписание содержать тот день, в которое оно присылается \r\n" +
            "Например, если вы выбрали рассылку на 2 дня каждый день и выбрали первый вариант - " +
            "расписание будет содержать пары на 'сегодня' и 'завтра'. В ином случае - на 'завтра' и 'послезавтра'. ",
            replyMarkup: CustomKeyboardStorage.IncludeTodayKeyboard
            );
    }
    public async Task<Message> UpdateWithWeeklyScheduleOptionsKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "В какой день вы хотите получать недельное расписание?",
            replyMarkup: CustomKeyboardStorage.WeeklyScheduleOptionsKeyboard
            );
    }
    public async Task<Message> UpdateWithSuccessMessage(
        SubscriptionSettings settings,
        CallbackQuery callback)
    {
        var formattedSettings = _settingsFormatter.Format(settings);

        return await _bot.EditMessageTextAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "*Настройки подписки успешно сохранены\\!* " +
            "\r\n Теперь они выглядят так: \r\n\r\n" + formattedSettings,
            replyMarkup: CustomKeyboardStorage.SubscriptionEnabledKeyboard
            );
    }
    public async Task<Message> UpdateWithCustomTextAndKeyboard(
        CallbackQuery callback,
        string text,
        InlineKeyboardMarkup keyboard)
    {
        return await _bot.EditMessageTextAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: text,
            replyMarkup: keyboard
            );
    }

    public async Task<Message> UpdateWithErrorMessage(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Возникла какая-то ошибка."
            );
    }


}
