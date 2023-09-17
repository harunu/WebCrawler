using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Data.Services;

namespace WebCrawler.Services
{

    public class CrawlerBackgroundService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CrawlerBackgroundService> _logger;
        private bool _isCrawling;
        private readonly object _lock = new object();
        private int _currentIndex = 1;

        public string Status { get; private set; } = "Not Started";

        public CrawlerBackgroundService(IServiceProvider serviceProvider, ILogger<CrawlerBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void StartCrawling()
        {
            lock (_lock)
            {
                if (_isCrawling)
                {
                    return;
                }

                Status = "Started";
                _isCrawling = true;
                _currentIndex = 1; // Resetting the index to 1
                _timer?.Change(TimeSpan.Zero, TimeSpan.FromMinutes(3)); // Reset the timer if it's already initialized
                _timer ??= new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
                _logger.LogInformation("Crawling started.");
            }
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation($"Crawling cycle {_currentIndex} started at {DateTime.UtcNow}");
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var crawlerService = scope.ServiceProvider.GetRequiredService<CrawlerService>();
                    await crawlerService.StartCrawling();
                }
                catch (ObjectDisposedException ex)
                {
                    _logger.LogError(ex, "An ObjectDisposedException occurred.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during crawling.");
                }
            }
            _logger.LogInformation($"Crawling cycle {_currentIndex} ended at {DateTime.UtcNow}");
            _currentIndex++; // Increment the index for the next cycle
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                Status = "Stopped";
                _timer?.Change(Timeout.Infinite, 0);
                _isCrawling = false;
                _logger.LogInformation("Crawling stopped.");
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}


