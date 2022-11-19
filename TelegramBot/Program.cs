
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ReaSchedule.DAL;
using ScheduleWorker.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBot;
using TelegramBot.Abstractions;
using TelegramBot.Services;

var builder = WebApplication.CreateBuilder(args);

var botConfig = builder.Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
var botToken = botConfig.BotToken ?? string.Empty;


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
builder.Services.AddSingleton<IBrowserWrapper, BrowserWrapper>();
builder.Services.AddSingleton<IMessageSender, MessageSender>();
builder.Services.AddScoped<IGroupSearchPipeline, GroupSearchPipeline>();
builder.Services.AddScoped<IArgumentExtractorService, ArgumentExtractorService>();
builder.Services.AddSingleton<ISpecificChainFactory, ChainFactory>();
builder.Services.AddScoped<IUserUpdater, UserUpdater>();
builder.Services.AddSingleton<ICallbackMessageUpdater, CallbackMessageUpdater>();


var app = builder.Build();

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
    Console.WriteLine(JsonConvert.SerializeObject(update));

    return Results.Ok();
})
.WithName("TelegramWebhook");

app.Run();
