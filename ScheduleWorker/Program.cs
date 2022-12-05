using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using ScheduleUpdateService.Services;
using Coravel;
using ScheduledActivities.Jobs;
using ScheduleUpdateService.Abstractions;
using ScheduleUpdateService.Extensions;
using System.Runtime.CompilerServices;
using Coravel.Scheduling.Schedule.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureServices((Action<HostBuilderContext, IServiceCollection>)((host, services) =>

    {
        services.AddDbContext<ScheduleDbContext>(options =>

            options.UseNpgsql(

                host.Configuration.GetConnectionString("DefaultConnection"),

                x => x.MigrationsAssembly("ReaSchedule.DAL")),

                ServiceLifetime.Singleton);


        services.AddHttpClient("Crawler", options =>
        {

            options.DefaultRequestHeaders.Add("User-Agent",

                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)" +

                " Chrome/102.0.4985.0 Safari/537.36 Edg/102.0.1235.1");
        });
        services.AddHostedService<ScheduleWorker.Worker>();
        services.AddTransient<TestDiJob>();
        services.AddTransient<UpdateGroupsScheduleJob>();
        services.AddScheduler();
        services.AddScheduleUpdateService(ServiceLifetime.Singleton);

    }))

    .Build();

host.Services
    .UseScheduler(scheduler =>
{
    scheduler
       .Schedule<UpdateGroupsScheduleJob>()
       .DailyAtHour(10);
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(14);
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(18);
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(22);
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(2);
    scheduler
        .Schedule<UpdateGroupsScheduleJob>()
        .DailyAtHour(6);

})
    .LogScheduledTaskProgress(host.Services.GetService<ILogger<IScheduler>>());



await host.RunAsync();

