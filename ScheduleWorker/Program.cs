using Microsoft.EntityFrameworkCore;

using ScheduleWorker;

using ReaSchedule.DAL;
using PuppeteerSharp;

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

        services.AddSingleton<IEntityConstructor, EntityConstructor>();

        services.AddHostedService<ScheduleWorker.Worker>();

        ServiceCollectionServiceExtensions.AddSingleton<ScheduleWorker.IBrowserWrapper, BrowserWrapper>(services);
        services.AddSingleton<IScheduleLoader, JsScheduleLoader>();
        services.AddSingleton<IParserPipeline, ParserPipeline>();

    }))

    .Build();



await host.RunAsync();

