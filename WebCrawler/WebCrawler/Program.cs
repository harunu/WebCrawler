using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebCrawler.Data.DBContext;
using WebCrawler.Data.Services;
using WebCrawler.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var dbContext = services.GetRequiredService<WebCrawlerContext>();

            if (dbContext != null)
            {
                await dbContext.EnsureDatabaseCreatedAsync();
            }
            else
            {
                throw new InvalidOperationException("DbContext could not be retrieved from the service provider.");
            }

            var crawlerService = services.GetRequiredService<CrawlerService>();

            if (crawlerService != null)
            {
                crawlerService.SetupDatabase();  // Set up the database first    
                                                 // await crawlerService.StartCrawling(); // Uncomment this if you want to start crawling immediately
            }
            else
            {
                throw new InvalidOperationException("CrawlerService could not be retrieved from the service provider.");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while setting up the database or starting the crawling process.");
        }

        await host.RunAsync();
    }



    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureServices((context, services) =>
            {

                var configuration = context.Configuration;
                var connectionString = configuration.GetConnectionString("DbConnectionString");

                services.AddDbContext<WebCrawlerContext>(options =>
                    options.UseSqlServer(connectionString));

                services.AddTransient<CrawlerService>();  // Changed from Scoped to Transient
                services.AddHostedService<CrawlerBackgroundService>();

                services.AddLogging();
                services.AddRazorPages();
                services.AddControllers();
            })
            .Configure((context, app) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }
                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });
            });
        });
}