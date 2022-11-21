using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService.Services;

public static class CustomKeyboardStorage
{
    private static readonly ReplyKeyboardMarkup _defaultReplyKeyboard = new(
                new[]
                {
                    new KeyboardButton[] { "Загрузить расписание" },
                    new KeyboardButton[] { "Настройки подписки", "Смена группы" },
                })
    {
        ResizeKeyboard = true
    };

    private static readonly List<InlineKeyboardButton> _subscriptionDisabledButton = new()
    { InlineKeyboardButton.WithCallbackData("Включить подписку", "Enable Subscription")};

    private static readonly List<List<InlineKeyboardButton>> _subscriptionEnabledButtons = new()
    {
        new(){ InlineKeyboardButton.WithCallbackData("Выключить подписку", "Disable Subscription") },
        new(){ InlineKeyboardButton.WithCallbackData("Поменять настройки подписки", "Change subscription settings") },
    };

    private static readonly List<List<InlineKeyboardButton>> _scheduleFrequencyButtons = new()
    {
        new() { InlineKeyboardButton.WithCallbackData("Каждый день", "Show Schedule: every day") },
        new() { InlineKeyboardButton.WithCallbackData("Каждую неделю", "Show Schedule: every week") }
    };

    private static readonly List<List<InlineKeyboardButton>> _daynNumberOptionButtons= new()
    {
        new() { InlineKeyboardButton.WithCallbackData("1 день", "Send every day for: 1 day") },
        new() { InlineKeyboardButton.WithCallbackData("2 дня", "Send every day for: 2 days") },
        new() { InlineKeyboardButton.WithCallbackData("3 дня", "Send every day for: 3 days") },
    };

    private static readonly List<List<InlineKeyboardButton>> _weeklyScheduleOptionButtons = new() {
        new()
        {
             InlineKeyboardButton.WithCallbackData("Понедельник", "Send every week on: Monday"),
             InlineKeyboardButton.WithCallbackData("Вторник", "Send every week on: Tuesday"),
        },
        new()
        {
            InlineKeyboardButton.WithCallbackData("Среду", "Send every week on: Wednesday"),
            InlineKeyboardButton.WithCallbackData("Четверг", "Send every week on: Thursday"),
        },
        new()
        {
            InlineKeyboardButton.WithCallbackData("Пятницу", "Send every week on: Friday"),
            InlineKeyboardButton.WithCallbackData("Субботу", "Send every week on: Saturday"),
        },
        new()
        {
            InlineKeyboardButton.WithCallbackData("Воскресенье", "Send every week on: Sunday"),
        }
    };



    public static ReplyKeyboardMarkup DefaultReplyKeyboard { get => _defaultReplyKeyboard; }
    public static InlineKeyboardMarkup SubscriptionDisabledKeyboard { get => new(_subscriptionDisabledButton); }
    public static InlineKeyboardMarkup SubscriptionEnabledKeyboard { get => new(_subscriptionEnabledButtons); }
    public static InlineKeyboardMarkup ScheduleFrequencyOptionsKeyboard { get => new(_scheduleFrequencyButtons); }
    public static InlineKeyboardMarkup DayNumberOptionsKeyboard { get => new(_daynNumberOptionButtons); }
    public static InlineKeyboardMarkup WeeklyScheduleOptionsKeyboard { get => new(_weeklyScheduleOptionButtons); }




}
