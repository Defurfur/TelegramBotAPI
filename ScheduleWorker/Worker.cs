namespace ScheduleWorker;



public class Worker : BackgroundService

{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private int count;

    public Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory)

    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)

    {

        using var scope = _scopeFactory.CreateScope();

        var hostLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

        try
        {
            _logger.LogInformation("Worker is working");


            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
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

    
}