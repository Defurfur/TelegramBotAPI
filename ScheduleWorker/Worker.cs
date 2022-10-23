using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using ReaSchedule.DAL;
using ReaSchedule.Models;

namespace ScheduleWorker

{

    public class Worker : BackgroundService

    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ScheduleDbContext _context;
        private readonly IEntityConstructor _scheduleParser;
        private readonly IScheduleLoader _scheduleLoader;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory scopeFactory,
            ScheduleDbContext context,
            IEntityConstructor scheduleParser,
            IScheduleLoader scheduleLoader
            )

        {

            _logger = logger;
            _scopeFactory = scopeFactory;
            _context = context;
            _scheduleParser = scheduleParser;
            _scheduleLoader = scheduleLoader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)

        {

            using var scope = _scopeFactory.CreateScope();

            var hostLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

            try

            {
                //await InternalExecuteAsync(stoppingToken); 
                await Parse(stoppingToken);
            }

            catch (TaskCanceledException tce)

            {
                _logger.LogWarning(tce, "Task was cancelled by user or system");
            }

            catch (Exception ex)

            {
                _logger.LogError(ex, "Unexpected end of execution");
            }

            hostLifetime!.StopApplication();

        }

        //private async Task InternalExecuteAsync(CancellationToken ct)

        //{
        //    var priceSourceList = await _context

        //        .PriceSource

        //        .Include(x => x.MaterialSupplier)

        //            .ThenInclude(x => x.Supplier)

        //        .Where(x => x.Enabled && x.FaultAttempts <= 3)

        //        .ToListAsync(cancellationToken: ct);



        //    var tasks = priceSourceList

        //        .Select(x => _service.UpdateAsync(x, ct));

        //    var results = await Task.WhenAll(tasks);

        //    var valids = results.Where(x => x.IsValid);

        //    var updatedSourceList = priceSourceList

        //        .Join(

        //            valids,

        //            x => x.Id,

        //            y => y.PriceSourceId,

        //            (x, y) =>

        //            {

        //                if (y is null || !y.IsValid)

        //                {

        //                    x.FaultAttempts++;

        //                    return x;

        //                }

        //                x!.MaterialSupplier!.Price = y.Price!.Value;

        //                return x;

        //            })

        //        .ToList();

        //    _context.UpdateRange(updatedSourceList);

        //    await _context.SaveChangesAsync(ct);
        //}

        private async Task Parse(CancellationToken ct)

        {

            var url = "https://rasp.rea.ru/?q=15.02%D0%B4-%D0%BC%D0%BC2%2F19%D0%B1";

            

            await _scheduleLoader.LoadPageContentAndParse(url, async classInfo =>
            {

                
            });


                //await page.EvaluateExpressionAsync("document.querySelector('#next').click(); {};");

                //var rAW = await page.EvaluateExpressionHandleAsync("document.ready");

                //await page.WaitForNavigationAsync(
                //    new NavigationOptions() {Timeout = 0 }
                //    );
                


        }

    }

}