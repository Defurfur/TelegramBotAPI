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
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "<b>Выберите, расписание какой недели вам присылать.</b> \r\n" +
            "Первый вариант позволяет получать расписание недели, содержащей день отправки. " +
            "Второй вариант - следующей после нее.",
            replyMarkup: CustomKeyboardStorage.WeeksToSendKeyboard
            );
    }
    public async Task<Message> UpdateWithTimeOfDayKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "<b>Выберите, в какое время суток присылать расписание</b>",
            replyMarkup: CustomKeyboardStorage.TimeOfDayKeyboard
            );
    }
    public async Task<Message> UpdateWithIncludeTodayKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "<b>Выберите должно ли сообщение с расписанием включать день отправки. \r\n</b>" +
            "При выборе 'нет' - каждый день вы будете получать расписание, начинающееся с 'завтра'.",
            replyMarkup: CustomKeyboardStorage.IncludeTodayKeyboard
            );
    }
    public async Task<Message> UpdateWithWeeklyScheduleOptionsKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "<b>В какой день вы хотите получать недельное расписание?</b>",
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
            "\r\nТеперь они выглядят так: \r\n\r\n" + formattedSettings,
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
