using Microsoft.EntityFrameworkCore;

using ScheduleWorker;

using ReaSchedule.DAL;
using PuppeteerSharp;

IHost host = Host.CreateDefaultBuilder(args)

    .ConfigureServices((host, services) =>

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

        services.AddSingleton<IEntityConstructor, EntityConstructor>();

        services.AddHostedService<ScheduleWorker.Worker>();

        services.AddSingleton<IBrowserWrapper, BrowserWrapper>();
        services.AddSingleton<IScheduleLoader, PuppeteerScheduleLoader>();

    })

    .Build();



await host.RunAsync();

