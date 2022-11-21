using Microsoft.EntityFrameworkCore;
using ReaSchedule.DAL;
using TelegramWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddDbContext<ScheduleDbContext>(options =>

           options.UseNpgsql(

               host.Configuration.GetConnectionString("DefaultConnection"),

               x => x.MigrationsAssembly("ReaSchedule.DAL")),

               ServiceLifetime.Singleton);
    })
    .Build();

host.Run();
