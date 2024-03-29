﻿using FluentCommandHandler;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotService.Abstractions;

namespace TelegramBotService.Services;

public class HandleUpdateService
{
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly IArgumentExtractorService _argumentExtractor;
    private readonly IFluentCommandHandler<ICommandArgs,ICommand<ICommandArgs, Task<Message>>> _handler;

    public HandleUpdateService(
        ILogger<HandleUpdateService> logger,
        IArgumentExtractorService argumentExtractor,
        IFluentCommandHandler<ICommandArgs, ICommand<ICommandArgs, Task<Message>>> handler)
    {
        _logger = logger;
        _argumentExtractor = argumentExtractor;
        _handler = handler;
    }

    public async Task EchoAsync(Update update)
    {
        if (update.Type == UpdateType.Message && (update.Message is null || update.Message.Text is null))
            return;

        if (update.Type != UpdateType.CallbackQuery && update.Type != UpdateType.Message)
            return;

        var args = _argumentExtractor.GetArgs(update);

        var command = await _handler.Process(args);

        try
        {
            if (command != null)
                await command.ExecuteAsync();
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
        }
    }


    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {updateType}", update.Type);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
    
}