
using Coravel;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduledActivities.Jobs;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Extensions;
using ScheduleUpdateService.Services;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Telegram.Bot;
using TelegramBotAPI;
using TelegramBotAPI.Middlewares;
using TelegramBotService;
using TelegramBotService.Abstractions;
using TelegramBotService.BackgroundTasks;
using TelegramBotService.Services;
using FluentCommandHandler;
using Telegram.Bot.Types;
using TelegramBotService.Models;
using TelegramBotService.Commands;
using Serilog;
using Serilog.Formatting;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

var logWriter = new StreamWriter("logs.txt");
Serilog.Debugging.SelfLog.Enable(logWriter);

var botConfig = builder.Configuration
    .GetSection("BotConfiguration")
    .Get<BotConfiguration>();

var botToken = botConfig?.BotToken ?? string.Empty;

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom
        .Configuration(context.Configuration);
});

builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddHttpClient("TelegramWebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botToken, httpClient));

builder.Services.AddDbContext<ScheduleDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("SQLServerExpress"),
                x => x.MigrationsAssembly("ReaSchedule.DAL")),
            ServiceLifetime.Scoped);

builder.Services.AddScoped<HandleUpdateService>();
builder.Services.AddScoped<IContextUpdateService, ContextUpdateService>();
builder.Services.AddScoped<IGroupSearchPipeline, GroupSearchPipeline>();
builder.Services.AddScoped<IArgumentExtractorService, ArgumentExtractorService>();
builder.Services.AddScoped<IUserUpdater, UserUpdater>();
builder.Services.AddScoped<IScheduleLoader, ScheduleLoader>();

builder.Services.AddSingleton<IMessageSender, MessageSender>();
builder.Services.AddSingleton<ISpecificChainFactory, ChainFactory>();
builder.Services.AddSingleton<ICallbackMessageUpdater, CallbackMessageUpdater>();
builder.Services.AddSingleton<IScheduleFormatter, ScheduleFormatter>();
builder.Services.AddSingleton<IUserSettingsFormatter, UserSettingsFormatter>();
builder.Services.AddSingleton<IChromiumKiller, ChromiumKiller>();

builder.Services.AddScheduleUpdateService(ServiceLifetime.Singleton);
builder.Services.AddQueue();
builder.Services.AddScheduler();


builder.Services.AddTransient<TryFindGroupAndChangeUser>();
builder.Services.AddTransient<TryFindGroupAndRegisterUser>();

builder.Services.AddTransient<SendDailyScheduleMorningJob>();
builder.Services.AddTransient<SendDailyScheduleAfternoonJob>();
builder.Services.AddTransient<SendDailyScheduleEveningJob>();

builder.Services.AddTransient<SendWeeklyScheduleMorningJob>();
builder.Services.AddTransient<SendWeeklyScheduleAfternoonJob>();
builder.Services.AddTransient<SendWeeklyScheduleEveningJob>();

builder.Services.AddTransient<UpdateGroupsScheduleJob>();
builder.Services.AddTransient<GlobalErrorHandlerMiddleware>();
builder.Services.AddFluentCommandHandler<ICommandArgs, ICommand<ICommandArgs, Task<Message>>>(options => 
    {
        options
            .AddRule(args =>
                   args.User != null
                && args.Callback != null
                && args.Callback.Data != null
                && args.CallbackMessageUpdater != null
                && args.OperationType == OperationType.SwitchWeekCallback
                && args.ScheduleLoader != null)
            .ForCommand(args => new SwitchWeekCallbackCommand(args));

        options
            .AddRule(args =>
                   args.User != null
                && args.User.SubscriptionSettings == null
                && args.Callback != null
                && args.Callback.Data == "Enable Subscription"
                && args.CallbackMessageUpdater != null
                && args.UserUpdater != null)
            .ForCommand(args => new CreateSettingsCallbackCommand(args));

        options
            .AddRule(args =>
                   args.User != null
                && args.User.SubscriptionSettings != null
                && args.Callback != null
                && args.Callback.Data == "Enable Subscription"
                && args.CallbackMessageUpdater != null
                && args.UserUpdater != null)
            .ForCommand(args => new EnableSubscriptionCommand(args));

        options
            .AddRule(args =>
                   args.User != null
                && args.User.SubscriptionSettings != null
                && args.Callback != null
                && args.Callback.Data == "Disable Subscription"
                && args.CallbackMessageUpdater != null
                && args.UserUpdater != null)
            .ForCommand(args => new DisableSubscriptionCommand(args));

        options
            .AddRule(args =>
                   args.User != null
                && args.User.SubscriptionSettings != null
                && args.Callback != null
                && args.Callback.Data != null
                && args.CallbackMessageUpdater != null
                && args.UserUpdater != null)
            .ForCommand(args => new ProcessCallbackCommand(args));

        options
            .AddRule(args =>
                   args.User != null
                && args.OperationType == OperationType.GroupChangeCommand
                && args.Update.Message != null)
            .ForCommand(args => new TryChangeGroupCommand(args));

        options
            .AddRule(args =>
                  args.Update.Message != null
               && args.User != null
               && args.MessageSender != null
               && args.OperationType == OperationType.ChangeSubscriptionSettingsRequest)
            .ForCommand(args => new ChangeSubscriptionSettingsCommand(args));

        options
            .AddRule(args =>
                  args.User != null
               && args.OperationType == OperationType.StartCommand
               && args.Update.Message != null)
            .ForCommand(args => new StartWithUserExistsCommand(args));

        options
           .AddRule(args =>
                  args.User == null
               && args.OperationType == OperationType.StartCommand
               && args.Update.Message != null)
           .ForCommand(args => new ShowStartMessageCommand(args));


        options
            .AddRule(args => 
                  args.OperationType        == OperationType.BugCommand
               && args.Update.Message       != null
               && args.ContextUpdateService != null)
            .ForCommand(args => new BugCommand(args));

        options
           .AddRule(args =>
                  args.User == null
               && args.OperationType == OperationType.Other
               && args.Update.Message != null)
           .ForCommand(args => new InvalidGroupInputCommand(args));

        options
            .AddRule(args =>
                  args.User           == null
               && args.OperationType  == OperationType.GroupInput
               && args.Update.Message != null)
            .ForCommand(args => new TryGetGroupCommand(args));

        options
            .AddRule(args =>
                  args.User           != null
               && args.OperationType  == OperationType.DownloadScheduleRequest
               && args.ScheduleLoader != null)
            .ForCommand(args => new DownloadScheduleCommand(args));

        options
            .AddRule(args =>
                  args.User             != null
               && args.OperationType    == OperationType.ChangeGroupButtonPressed)
            .ForCommand(args => new ChangeGroupButtonCommand(args));

    });

var app = builder.Build();


app.Services.UseScheduler(scheduler =>
{

    scheduler
        .Schedule<SendWeeklyScheduleMorningJob>()
        .DailyAtHour(4);
    //.RunOnceAtStart();

    scheduler
        .Schedule<SendWeeklyScheduleAfternoonJob>()
        .DailyAtHour(10);

    scheduler
        .Schedule<SendWeeklyScheduleEveningJob>()
        .DailyAtHour(16);

    scheduler
        .Schedule<SendDailyScheduleMorningJob>()
        .DailyAtHour(4);

    scheduler
        .Schedule<SendDailyScheduleAfternoonJob>()
        .DailyAtHour(10);

    scheduler
        .Schedule<SendDailyScheduleEveningJob>()
        .DailyAtHour(16);


    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .Hourly()
        .PreventOverlapping("Updater")
        .RunOnceAtStart();


});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseHttpLogging();
//app.UseMiddleware<GlobalErrorHandlerMiddleware>();

app.MapGet("/", () =>
{
    return Results.Ok("Everything works fine");
});


app.MapPost($"/bot/{botConfig.EscapedBotToken}", async (
    HttpRequest request,
    HandleUpdateService handleUpdateService,
    NewtonsoftJsonUpdate update) =>
{

    await handleUpdateService.EchoAsync(update);

    return Results.Ok();
})
.WithName("TelegramWebhook");

app.Lifetime.ApplicationStopped.Register(() => logWriter.Dispose(), true);

app.Run();
