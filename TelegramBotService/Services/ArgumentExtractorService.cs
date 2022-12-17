﻿using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotService.Abstractions;
using TelegramBotService.Models;
using User = ReaSchedule.Models.User;

namespace TelegramBotService.Services;

public class ArgumentExtractorService : IArgumentExtractorService
{
    private readonly ScheduleDbContext _context;
    private readonly IMessageSender _messageSender;
    private readonly IGroupSearchPipeline _groupSearchPipeline;
    private readonly ICallbackMessageUpdater _callbackMessageUpdater;
    private readonly IUserUpdater _userUpdater;
    private readonly IScheduleLoader _scheduleLoader;
    private readonly IContextUpdateService _contextUpdateService;

    public ArgumentExtractorService(
        ScheduleDbContext context,
        IMessageSender messageSender,
        IGroupSearchPipeline groupSearchPipeline,
        ICallbackMessageUpdater callbackMessageUpdater,
        IUserUpdater userUpdater,
        IScheduleLoader scheduleLoader,
        IContextUpdateService contextUpdateService)
    {
        _context = context;
        _messageSender = messageSender;
        _groupSearchPipeline = groupSearchPipeline;
        _callbackMessageUpdater = callbackMessageUpdater;
        _userUpdater = userUpdater;
        _scheduleLoader = scheduleLoader;
        _contextUpdateService = contextUpdateService;
    }

    public ICommandArgs GetArgs(Update update)
    {
        ArgumentNullException.ThrowIfNull(update);

        ICommandArgs commandArgs = new CommandArgs() { Update = update };

        if(update.Message is not null || update.CallbackQuery is not null)
        {
            var chatId = update.Message == null 
                ? update.CallbackQuery!.Message!.Chat.Id 
                : update.Message.Chat.Id;

            commandArgs.User = TryGetUser(chatId);
        }

        commandArgs.MessageSender = _messageSender;

        commandArgs.Callback = update.CallbackQuery;

        commandArgs.UpdateType = update.Type;

        commandArgs.GroupSearchPipeline = _groupSearchPipeline;

        if (commandArgs.Update.Message != null 
            && commandArgs.Update.Message.Text != null
            && commandArgs.Update.Message.Text.Contains("/bug")
           )
        {
            commandArgs.OperationType = OperationType.BugCommand;
            commandArgs.ContextUpdateService = _contextUpdateService;
        }

        if (commandArgs.User == null
            && commandArgs.UpdateType == UpdateType.Message
            && TryGetStartCommand(update))
        {
            commandArgs.OperationType = OperationType.StartCommand;
        }


        if (commandArgs.User == null
            && commandArgs.UpdateType == UpdateType.Message
            && update.Message!.Text != null
            && CheckMessageForGroupInput(update.Message.Text))
        {
            commandArgs.OperationType = OperationType.GroupInput;
        }

        if(commandArgs.User != null && update.Message != null)
        {
            commandArgs.OperationType = update.Message.Text switch
            {
                "Загрузить расписание" => OperationType.DownloadScheduleRequest,
                "Смена группы" => OperationType.ChangeGroupButtonPressed,
                "Настройки подписки" => OperationType.ChangeSubscriptionSettingsRequest,
                _ => commandArgs.OperationType
            };
        }
        if (commandArgs.User != null
            && update.Message != null
            && update.Message.Text!.Split(' ')[0] == "/change"
            && update.Message.Text!.Split(' ').Length == 2
            && CheckMessageForGroupInput(update.Message.Text.Split(' ')[1]))
        {
            commandArgs.OperationType = OperationType.GroupChangeCommand;
            commandArgs.Update.Message!.Text = update.Message.Text.Replace("/change ", "").Trim();
        }

        if (commandArgs.OperationType == OperationType.DownloadScheduleRequest)
            commandArgs.ScheduleLoader = _scheduleLoader;

        commandArgs = ProcessCallback(commandArgs, update);




        return commandArgs;
    }
    private User? TryGetUser(long id)
    {
        var user = _context
            .Users
            .Include(x => x.SubscriptionSettings)
            .FirstOrDefault(x => x.ChatId == id);
        if (user == default)
            return null;
        return user;
    }
    private bool TryGetStartCommand(Update update)
    {
        if (update.Message!.Text == null)
            return false;
        return update.Message.Text.Split(' ')[0] == "/start";
    }
    private bool CheckMessageForGroupInput(string text)
    {
        var cleanedText = text.ToLower().Trim();

        bool firstCondition =
            !cleanedText.Contains(' ')
            && cleanedText.Contains('.')
            && cleanedText.Contains('/')
            && cleanedText.Contains('-')
            && cleanedText.Length >= 13
            && cleanedText.Length <= 19;

        bool secondCondition = cleanedText.StartsWith("97в/") || cleanedText.StartsWith("97з/");


        return firstCondition || secondCondition;
    }
    private ICommandArgs ProcessCallback(ICommandArgs args, Update update)
    {
        args.Callback = update.CallbackQuery;

        if (args.Callback is null)
            return args;

        if (args.Callback.Data.Contains("ScheduleSwitcher"))
        {
            args.OperationType = OperationType.SwitchWeekCallback;
            args.ScheduleLoader = _scheduleLoader;
        }

        args.CallbackMessageUpdater = _callbackMessageUpdater;
        args.UserUpdater = _userUpdater;

        return args;


    }

}