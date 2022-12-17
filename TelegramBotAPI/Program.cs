
using Coravel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReaSchedule.DAL;
using ReaSchedule.Models;
using ScheduledActivities.Jobs;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Extensions;
using ScheduleUpdateService.Services;
using Telegram.Bot;
using TelegramBotService;
using TelegramBotService.Abstractions;
using TelegramBotService.BackgroundTasks;
using TelegramBotService.Services;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
var botToken = botConfig.BotToken ?? string.Empty;

builder.Logging.AddEventLog();

builder.Services.AddHostedService<ConfigureWebhook>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("TelegramWebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botToken, httpClient));

builder.Services.AddDbContext<ScheduleDbContext>(options =>

            options.UseNpgsql(

                builder.Configuration.GetConnectionString("DefaultConnection"),

                x => x.MigrationsAssembly("ReaSchedule.DAL")),

                ServiceLifetime.Scoped);

builder.Services.AddScoped<HandleUpdateService>();
builder.Services.AddScheduleUpdateService(ServiceLifetime.Singleton);
builder.Services.AddQueue();
builder.Services.AddScheduler();
builder.Services.AddSingleton<IBrowserWrapper, BrowserWrapper>();
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
builder.Services.AddTransient<TryFindGroupAndChangeUserInvocable>();
builder.Services.AddTransient<TryFindGroupAndRegisterUserInvocable>();
builder.Services.AddTransient<SendScheduleToSubsDailyJob>();
builder.Services.AddTransient<SendScheduleToSubsWeeklyJob>();



var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler
    .ScheduleWithParams<SendScheduleToSubsDailyJob>(TimeOfDay.Morning)
    .DailyAtHour(7);

    scheduler
    .ScheduleWithParams<SendScheduleToSubsDailyJob>(TimeOfDay.Afternoon)
    .DailyAtHour(13);
    //.EveryTenMinutes();

    scheduler
    .ScheduleWithParams<SendScheduleToSubsDailyJob>(TimeOfDay.Evening)
    .DailyAtHour(19);
    
    scheduler
    .ScheduleWithParams<SendScheduleToSubsWeeklyJob>(TimeOfDay.Morning)
    .DailyAtHour(7);

    scheduler
    .ScheduleWithParams<SendScheduleToSubsWeeklyJob>(TimeOfDay.Afternoon)
    .EveryTenMinutes();
    //.DailyAtHour(13);

    scheduler
    .ScheduleWithParams<SendScheduleToSubsWeeklyJob>(TimeOfDay.Evening)
    .DailyAtHour(19);

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
