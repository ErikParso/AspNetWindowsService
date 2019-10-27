using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientManagerService.Services.QueuedTasksService
{
    /// <summary>
    /// Service executing tasks stored in <see cref="BackgroundTaskQueue"/>.
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private CancellationTokenSource _shutdown;
        private Task _backgroundTask;
        private readonly ILogger<QueuedHostedService> _logger;

        /// <summary>
        /// Initializes <see cref="QueuedHostedService"/>
        /// </summary>
        /// <param name="taskQueue">Background task queue.</param>
        /// <param name="logger">Logger.</param>
        public QueuedHostedService(
            IBackgroundTaskQueue taskQueue,
            ILogger<QueuedHostedService> logger)
        {
            TaskQueue = taskQueue;
            _logger = logger;
            _shutdown = new CancellationTokenSource();
        }

        /// <summary>
        /// Background tasks queue.
        /// </summary>
        public IBackgroundTaskQueue TaskQueue { get; }

        /// <summary>
        /// Starts tasks processing cycle.
        /// </summary>
        /// <param name="stoppingToken">Cancelation token.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            _backgroundTask = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await BackgroundProcessing();
                }
            }, stoppingToken);

            await _backgroundTask;
        }

        private async Task BackgroundProcessing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueAsync(_shutdown.Token);

                try
                {
                    await workItem(_shutdown.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        /// <summary>
        /// Stops cycle processing background tasks.
        /// </summary>
        /// <param name="stoppingToken">Cancelation token.</param>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            _shutdown.Cancel();

            await Task.WhenAny(_backgroundTask,
                    Task.Delay(Timeout.Infinite, stoppingToken));
        }
    }
}
