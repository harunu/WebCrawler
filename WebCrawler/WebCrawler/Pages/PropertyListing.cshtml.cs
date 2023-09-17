using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebCrawler.Data.DBContext;
using WebCrawler.Data.Models;

namespace WebCrawler.Pages
{
    public class PropertyListingModel : PageModel
    {
        private readonly WebCrawlerContext _context;

        public PropertyListingModel(WebCrawlerContext context)
        {
            _context = context;
        }

        public IList<PropertyListing> PropertyListings { get; set; }

        public async Task OnGetAsync()
        {
            PropertyListings = await _context.PropertyListings.ToListAsync();
        }
    }
}
