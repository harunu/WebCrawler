using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebCrawler.Data.DBContext;
using WebCrawler.Data.Interfaces;
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
            webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                if (env.EnvironmentName == "Docker")
                {
                    config.AddJsonFile("appsettings.Docker.json", optional: true, reloadOnChange: true);
                }
            })
            .ConfigureServices((context, services) =>
            {
                string useSqlite = context.Configuration.GetSection("UseSqlite").Value;
                if (string.Equals(useSqlite, "true", StringComparison.OrdinalIgnoreCase))
                {
                    services.AddDbContext<WebCrawlerContext>(options =>
                        options.UseSqlite("Data Source=webcrawler.db"));
                }
                else
                {
                    var configuration = context.Configuration;
                    var connectionString = configuration.GetConnectionString("DbConnectionString");
                    services.AddDbContext<WebCrawlerContext>(options =>
                    options.UseSqlServer(connectionString));
                }

                services.AddTransient<CrawlerService>();
                services.AddHostedService<CrawlerBackgroundService>();

                services.AddScoped<IHttpClientService, HttpClientService>();
                services.AddScoped<IHtmlParserService, HtmlParserService>();
                services.AddScoped<IDatabaseService, DatabaseService>();

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
