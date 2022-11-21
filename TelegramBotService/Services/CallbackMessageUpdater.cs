using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services;

public class CallbackMessageUpdater : ICallbackMessageUpdater
{
    private readonly ITelegramBotClient _bot;

    public CallbackMessageUpdater(
        ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task<Message> UpdateWithScheduleFrequencyOptionsKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Выберите интервал, по котороу будет присылаться расписание",
            replyMarkup: CustomKeyboardStorage.ScheduleFrequencyOptionsKeyboard
            );
    }
    public async Task<Message> UpdateWithDayNumberOptionsKeyboard(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Выберите количество дней с расписанием, которые вы будете получать каждый день." +
            "\r\n Например, 1 день - каждый день вы будете получать расписание на завтра." +
            "\r\n 2 дня - расписание на завтра и послезавтра.",
            replyMarkup: CustomKeyboardStorage.DayNumberOptionsKeyboard
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
    public async Task<Message> UpdateWithSuccessMessage(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Настройки подписки успешно сохранены!"
            );
    }
    public async Task<Message> UpdateWithSubscriptionDisabled(CallbackQuery callback)
    {
        return await _bot.EditMessageTextAsync(
            chatId: callback.Message.Chat.Id,
            messageId: callback.Message.MessageId,
            text: "Подписка отменена!"
            );
    }


}
