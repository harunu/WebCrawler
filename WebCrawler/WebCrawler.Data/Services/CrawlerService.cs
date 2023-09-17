using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Data.Configuration;
using WebCrawler.Data.DBContext;
using WebCrawler.Data.Interfaces;
using WebCrawler.Data.Models;

namespace WebCrawler.Data.Services
{
   
    public class CrawlerService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IHtmlParserService _htmlParserService;
        private readonly IDatabaseService _databaseService;
        private readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5);  

        public CrawlerService(IHttpClientService httpClientService, IHtmlParserService htmlParserService, IDatabaseService databaseService, IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;
            _htmlParserService = htmlParserService;
            _databaseService = databaseService;
            _serviceProvider = serviceProvider;
        }

        public void SetupDatabase()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WebCrawlerContext>();
            _databaseService.SetupDatabase(context);
        }

        public async Task StartCrawling()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WebCrawlerContext>();

            var baseUrl = CrawlerConfiguration.BaseUrl;
            var response = await _httpClientService.GetStringAsync(baseUrl);

            var jsonData = _htmlParserService.GetJsonDataFromHtml(response);
            if (jsonData != null)
            {
                var listingsToken = _htmlParserService.GetPropertyListingsFromJsonData(jsonData);
                var listings = await CreatePropertyListings(listingsToken, baseUrl);

                await _databaseService.SaveListingsAsync(context, listings);
            }
        }

        private async Task<IEnumerable<PropertyListing>> CreatePropertyListings(JToken listingsToken, string baseUrl)
        {
            var listings = new List<PropertyListing>();
            var tasks = new List<Task>();

            foreach (var listing in listingsToken)
            {
                var propertyListing = _htmlParserService.CreatePropertyListingFromJson(listing);
                tasks.Add(GetAndSetDetailedInfo(baseUrl, listing, propertyListing));
                listings.Add(propertyListing);
            }

            await Task.WhenAll(tasks);
            return listings;
        }

        private async Task GetAndSetDetailedInfo(string baseUrl, JToken listing, PropertyListing propertyListing)
        {
            await _semaphore.WaitAsync();

            try
            {
                var listingId = listing["id"]?.ToString();

                if (listingId != null)
                {
                    var detailUrl = baseUrl + listingId;
                    var detailResponse = await _httpClientService.GetAsync(detailUrl);

                    if (detailResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        var detailResponseContent = await detailResponse.Content.ReadAsStringAsync();
                        var detailJsonObject = _htmlParserService.GetDetailJsonObject(detailResponseContent);
                        var detailHtmlDocument = new HtmlAgilityPack.HtmlDocument();
                        detailHtmlDocument.LoadHtml(detailResponseContent);

                        var imageUrl = _htmlParserService.GetImageUrl(detailHtmlDocument);
                        _htmlParserService.UpdatePropertyListingWithDetailInfo(detailJsonObject, propertyListing, imageUrl);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}


