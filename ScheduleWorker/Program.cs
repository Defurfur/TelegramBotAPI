using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using PuppeteerSharp;
using ReaSchedule.Models;
using ScheduleWorker.Services;

IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureServices((Action<HostBuilderContext, IServiceCollection>)((host, services) =>

    {

        services.AddDbContext<ScheduleDbContext>(options =>

            options.UseNpgsql(

                host.Configuration.GetConnectionString("DefaultConnection"),

                x => x.MigrationsAssembly("ReaSchedule.DAL")),

                ServiceLifetime.Singleton);



        //services.AddHttpClient("Crawler", options =>

        //{

        //    options.DefaultRequestHeaders.Add("User-Agent",

        //        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)" +

        //        " Chrome/102.0.4985.0 Safari/537.36 Edg/102.0.1235.1");

        //});

        services.AddHttpClient("Crawler", options =>
        {

            options.DefaultRequestHeaders.Add("User-Agent",

                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)" +

                " Chrome/102.0.4985.0 Safari/537.36 Edg/102.0.1235.1");
        });

        services.AddSingleton<IEntityUpdater, EntityUpdater>();

        services.AddHostedService<ScheduleWorker.Worker>();

        services.AddSingleton<IBrowserWrapper, BrowserWrapper>();
        services.AddSingleton<IScheduleLoader, JsScheduleLoader>();
        services.AddSingleton<IParserPipeline, ParserPipeline>();
        services.AddSingleton<IReaClassFactory, SimpleReaClassFactory>();
        services.AddSingleton<IScheduleWeekFactory, ScheduleWeekFactory>();
        services.AddSingleton<IReaGroupFactory, ReaGroupFactory>();
        services.AddSingleton<IHashingService, HashingService>();

    }))

    .Build();



await host.RunAsync();

