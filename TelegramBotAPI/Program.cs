
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

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
var botToken = botConfig?.BotToken ?? string.Empty;

builder.Logging.AddEventLog();

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
    //scheduler
    //    .Schedule<UpdateGroupsScheduleJob>()
    //    .EveryTenMinutes()
    //    //.DailyAtHour(10)
    //    .PreventOverlapping("Updater")
    //    .RunOnceAtStart();
    //scheduler
    //    .Schedule<UpdateGroupsScheduleJob>()
    //    .DailyAtHour(14)
    //    .PreventOverlapping("Updater");
    //scheduler
    //    .Schedule<UpdateGroupsScheduleJob>()
    //    .DailyAtHour(18)
    //    .PreventOverlapping("Updater");
    //scheduler
    //    .Schedule<UpdateGroupsScheduleJob>()
    //    .DailyAtHour(22)
    //    .PreventOverlapping("Updater");
    //scheduler
    //    .Schedule<UpdateGroupsScheduleJob>()
    //    .DailyAtHour(2)
    //    .PreventOverlapping("Updater");
    //scheduler
    //    .Schedule<UpdateGroupsScheduleJob>()
    //    .DailyAtHour(6)
    //    .PreventOverlapping("Updater");

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

app.Run();
