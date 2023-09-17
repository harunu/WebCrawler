using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data.DBContext;
using WebCrawler.Data.Models;

namespace WebCrawler.Data.Interfaces
{
    public interface IDatabaseService
    {
        void SetupDatabase(WebCrawlerContext context);
        Task SaveListingsAsync(WebCrawlerContext context, IEnumerable<PropertyListing> listings);
    }
}
