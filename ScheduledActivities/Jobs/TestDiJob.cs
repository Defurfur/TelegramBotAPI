using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace ScheduledActivities.Jobs
{
    public class TestDiJob : IInvocable
    {
        private readonly ILogger<TestDiJob> _logger;

        public TestDiJob(ILogger<TestDiJob> logger)
        {
            _logger = logger;
        }

        public Task Invoke()
        {
            _logger.LogInformation($"Something has happened at {TimeOnly.FromDateTime(DateTime.Now)} ");
            return Task.CompletedTask;
        }
    }
}
