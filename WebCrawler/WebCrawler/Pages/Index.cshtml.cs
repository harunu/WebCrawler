using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebCrawler.Services;

namespace WebCrawler.Pages
{

    public class IndexModel : PageModel
    {
        private readonly CrawlerBackgroundService _crawlerBackgroundService;

        public IndexModel(IEnumerable<IHostedService> hostedServices)
        {
            _crawlerBackgroundService = hostedServices.OfType<CrawlerBackgroundService>().FirstOrDefault();
        }

        public string Status => _crawlerBackgroundService?.Status;

        public IActionResult OnPostStart()
        {
            _crawlerBackgroundService?.StartCrawling();
            return RedirectToPage();
        }

        public IActionResult OnPostStop()
        {
            _crawlerBackgroundService?.StopAsync(CancellationToken.None).Wait();
            return RedirectToPage();
        }
    }

}