using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data.DBContext;
using WebCrawler.Data.Interfaces;
using WebCrawler.Data.Models;
using System.Threading.Tasks;


namespace WebCrawler.Data.Services
{
    //public class DatabaseService : IDatabaseService
    //{
    //    public void SetupDatabase(WebCrawlerContext context)
    //    {
    //        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('PropertyListings', RESEED, 0)");
    //        context.PropertyListings.RemoveRange(context.PropertyListings);
    //        context.SaveChanges();
    //    }

    //    public async Task SaveListingsAsync(WebCrawlerContext context, IEnumerable<PropertyListing> listings)
    //    {
    //        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('PropertyListings', RESEED, 0)");
    //        context.PropertyListings.RemoveRange(context.PropertyListings);
    //        await context.PropertyListings.AddRangeAsync(listings);
    //        await context.SaveChangesAsync();
    //    }
    //}

    public class DatabaseService : IDatabaseService
    {
        public void SetupDatabase(WebCrawlerContext context)
        {
            if (context.Database.IsSqlServer())
            {
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('PropertyListings', RESEED, 0)");
                context.PropertyListings.RemoveRange(context.PropertyListings);
                context.SaveChanges();
            }
            else
            {
                // equivalent SQLite command here.
                context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'PropertyListings';");
                context.PropertyListings.RemoveRange(context.PropertyListings);
                context.SaveChanges();
            }

        }

        public async Task SaveListingsAsync(WebCrawlerContext context, IEnumerable<PropertyListing> listings)
        {
            if (context.Database.IsSqlServer())
            {
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('PropertyListings', RESEED, 0)");
                context.PropertyListings.RemoveRange(context.PropertyListings);
                await context.PropertyListings.AddRangeAsync(listings);
                await context.SaveChangesAsync();
            }
            else
            {
                context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name = 'PropertyListings';");
                context.PropertyListings.RemoveRange(context.PropertyListings);
                await context.PropertyListings.AddRangeAsync(listings);
                await context.SaveChangesAsync();
            }
        }
    }

}
