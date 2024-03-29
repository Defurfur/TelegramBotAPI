﻿using Telegram.Bot;
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

    private static readonly List<InlineKeyboardButton> _noSubscriptionButton = new()
    { InlineKeyboardButton.WithCallbackData("Включить подписку", "Enable Subscription")};

    private static readonly List<List<InlineKeyboardButton>> _subscriptionEnabledButtons = new()
    {
        new(){ InlineKeyboardButton.WithCallbackData("Выключить подписку", "Disable Subscription") },
        new(){ InlineKeyboardButton.WithCallbackData("Поменять настройки подписки", "Change subscription settings") },
    };

    private static readonly List<List<InlineKeyboardButton>> _subscriptionDisabledButtons = new()
    {
        new(){ InlineKeyboardButton.WithCallbackData("Включить подписку", "Enable Subscription") },
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

    private static readonly List<InlineKeyboardButton> _weekScheduleSwitchersSetOnOne = new()
    {
        InlineKeyboardButton.WithCallbackData("~1~", "ScheduleSwitchers: 1"),
        InlineKeyboardButton.WithCallbackData("2", "ScheduleSwitchers: 2"),
    };
    private static readonly List<InlineKeyboardButton> _weekScheduleSwitchersSetOnTwo = new()
    {
        InlineKeyboardButton.WithCallbackData("1", "ScheduleSwitchers: 1"),
        InlineKeyboardButton.WithCallbackData("~2~", "ScheduleSwitchers: 2"),
    };
    private static readonly List<InlineKeyboardButton> _weekToSendButtons = new()
    {
        InlineKeyboardButton.WithCallbackData("Текущей недели", "WeekToSend: Current"),
        InlineKeyboardButton.WithCallbackData("Следующей недели", "WeekToSend: Next"),
    };
    private static readonly List<List<InlineKeyboardButton>> _timeOfDayButtons = new()
    {
        new()
        {
            InlineKeyboardButton.WithCallbackData("Утром (до 8:00)", "TimeOfDay: Morning"),
            InlineKeyboardButton.WithCallbackData("Днем (до 14:00)", "TimeOfDay: Afternoon"),
        },
        new()
        {
            InlineKeyboardButton.WithCallbackData("Вечером (до 20:00)", "TimeOfDay: Evening"),
        }
    };
    private static readonly List<InlineKeyboardButton> _includeTodayButtons = new()
    {
        InlineKeyboardButton.WithCallbackData("Да", "IncludeToday: True"),
        InlineKeyboardButton.WithCallbackData("Нет", "IncludeToday: False"),
    };



    public static ReplyKeyboardMarkup DefaultReplyKeyboard { get => _defaultReplyKeyboard; }
    public static InlineKeyboardMarkup NoSubscriptionKeyboard { get => new(_noSubscriptionButton); }
    public static InlineKeyboardMarkup SubscriptionEnabledKeyboard { get => new(_subscriptionEnabledButtons); }
    public static InlineKeyboardMarkup SubscriptionDisabledKeyboard { get => new(_subscriptionDisabledButtons); }
    public static InlineKeyboardMarkup ScheduleFrequencyOptionsKeyboard { get => new(_scheduleFrequencyButtons); }
    public static InlineKeyboardMarkup DayNumberOptionsKeyboard { get => new(_daynNumberOptionButtons); }
    public static InlineKeyboardMarkup WeeklyScheduleOptionsKeyboard { get => new(_weeklyScheduleOptionButtons); }
    public static InlineKeyboardMarkup WeekScheduleSwitchersSetOnOne { get => new(_weekScheduleSwitchersSetOnOne); }
    public static InlineKeyboardMarkup WeekScheduleSwitchersSetOnTwo { get => new(_weekScheduleSwitchersSetOnTwo); }
    public static InlineKeyboardMarkup WeeksToSendKeyboard { get => new(_weekToSendButtons); }
    public static InlineKeyboardMarkup TimeOfDayKeyboard { get => new(_timeOfDayButtons); }
    public static InlineKeyboardMarkup IncludeTodayKeyboard { get => new(_includeTodayButtons); }
}
