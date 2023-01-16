
using Coravel;
using Microsoft.AspNetCore.Components.Web;
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
using Telegram.Bot;
using TelegramBotAPI;
using TelegramBotService;
using TelegramBotService.Abstractions;
using TelegramBotService.BackgroundTasks;
using TelegramBotService.Services;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
var botToken = botConfig.BotToken ?? string.Empty;

Console.WriteLine(JsonConvert.SerializeObject(botConfig));
Console.WriteLine( Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

builder.Logging.AddEventLog();

builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("TelegramWebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botToken, httpClient));

builder.Services.AddDbContext<ScheduleDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("SQLServerExpress"),
                x => x.MigrationsAssembly("ReaSchedule.DAL")),
                ServiceLifetime.Scoped);
builder.Services.AddScoped<HandleUpdateService>();
builder.Services.AddScheduleUpdateService(ServiceLifetime.Singleton);
builder.Services.AddQueue();
builder.Services.AddScheduler();
builder.Services.AddScoped<IContextUpdateService, ContextUpdateService>();
builder.Services.AddSingleton<IMessageSender, MessageSender>();
builder.Services.AddScoped<IGroupSearchPipeline, GroupSearchPipeline>();
builder.Services.AddScoped<IArgumentExtractorService, ArgumentExtractorService>();
builder.Services.AddSingleton<ISpecificChainFactory, ChainFactory>();
builder.Services.AddScoped<IUserUpdater, UserUpdater>();
builder.Services.AddSingleton<ICallbackMessageUpdater, CallbackMessageUpdater>();
builder.Services.AddScoped<IScheduleLoader, ScheduleLoader>();
builder.Services.AddSingleton<IScheduleFormatter, ScheduleFormatter>();
builder.Services.AddSingleton<IUserSettingsFormatter, UserSettingsFormatter>();
builder.Services.AddTransient<TryFindGroupAndChangeUser>();
builder.Services.AddTransient<TryFindGroupAndRegisterUser>();
builder.Services.AddTransient<SendScheduleToSubsDailyJob>();
builder.Services.AddTransient<SendScheduleToSubsWeeklyJob>();
builder.Services.AddTransient<UpdateGroupsScheduleJob>();

//ScheduleLoader : Sc > ScheduleFormatter : St AND ContextUpdateServicce : Sc > DbContext : Sc
//IMessageSender : St > UserSettingsFormatter : St

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{

    scheduler
        .Schedule<SendScheduleToSubsDailyJob>()
        .DailyAtHour(4);

    scheduler
        .Schedule<SendScheduleToSubsDailyJob>()
        .DailyAtHour(10);
        //.EveryTenMinutes();

    scheduler
        .Schedule<SendScheduleToSubsDailyJob>()
        .DailyAtHour(16);

    scheduler
        .ScheduleWithParams<SendScheduleToSubsWeeklyJob>(TimeOfDay.Morning)
        .DailyAtHour(4);

    scheduler
        .ScheduleWithParams<SendScheduleToSubsWeeklyJob>(TimeOfDay.Afternoon)
        .DailyAtHour(10);

    scheduler
        .ScheduleWithParams<SendScheduleToSubsWeeklyJob>(TimeOfDay.Evening)
        .DailyAtHour(16);


    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(10)
        .PreventOverlapping("Updater");
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(14)
        .PreventOverlapping("Updater");
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(18)
        .PreventOverlapping("Updater");
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(22)
        .PreventOverlapping("Updater");
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(2)
        .PreventOverlapping("Updater");
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(6)
        .PreventOverlapping("Updater");

});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHttpLogging();


app.MapPost($"/bot/{botConfig.EscapedBotToken}", async (
    ITelegramBotClient botClient,
    HttpRequest request,
    HandleUpdateService handleUpdateService,
    NewtonsoftJsonUpdate update) =>
{
    await handleUpdateService.EchoAsync(update);

    return Results.Ok();
})
.WithName("TelegramWebhook");

app.Run();
