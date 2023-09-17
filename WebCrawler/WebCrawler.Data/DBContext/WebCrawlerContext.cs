using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data.Models;

namespace WebCrawler.Data.DBContext
{
    public class WebCrawlerContext : DbContext
    {
        public WebCrawlerContext(DbContextOptions<WebCrawlerContext> options)
            : base(options)
        {
        }

        public DbSet<PropertyListing> PropertyListings { get; set; }

        public async Task EnsureDatabaseCreatedAsync()
        {
            if (!(await this.Database.CanConnectAsync()))
            {
                // If the database does not exist, create it
                await this.Database.EnsureCreatedAsync();
            }
        }
    }
}
