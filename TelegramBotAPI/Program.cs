
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

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>()!;
var botToken = botConfig.BotToken ?? string.Empty;

builder.Logging.AddEventLog();

builder.Services.AddHostedService<ConfigureWebhook>();

// review - use Swagger only if needed. 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("TelegramWebhook")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botToken, httpClient));

builder.Services.AddDbContext<ScheduleDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")!,
                x => x.MigrationsAssembly("ReaSchedule.DAL")),
                ServiceLifetime.Scoped);

// review - watch inside
// review - check injection doubling and sort injections - here some empty space separations are ok 
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
builder.Services.AddTransient<UpdateGroupsScheduleJob>();


// review - warning - separate into service provider extension elsewhere and use here.
// Programm cs should be as simple and obvious as possible
var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler
    .ScheduleWithParams<SendScheduleToSubsDailyJob>(TimeOfDay.Morning)
    .DailyAtHour(4);

    scheduler
    .ScheduleWithParams<SendScheduleToSubsDailyJob>(TimeOfDay.Afternoon)
    .DailyAtHour(10);
    //.EveryTenMinutes();

    scheduler
    .ScheduleWithParams<SendScheduleToSubsDailyJob>(TimeOfDay.Evening)
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
      .PreventOverlapping("Updater")
      .RunOnceAtStart();
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
