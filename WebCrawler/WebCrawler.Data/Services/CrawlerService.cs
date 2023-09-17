using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data.DBContext;
using WebCrawler.Data.Models;

namespace WebCrawler.Data.Services
{
    public class CrawlerService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private readonly WebCrawlerContext _context;

        public CrawlerService(WebCrawlerContext context)
        {
            _context = context;
        }

        public void SetupDatabase()
        {
            _context.PropertyListings.RemoveRange(_context.PropertyListings);
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('PropertyListings', RESEED, 0)");
            _context.SaveChanges();
        }


        public async Task StartCrawling()
        {

            var baseUrl = "https://crawling-coding-challenge-properti-ag.vercel.app/";
            var response = await httpClient.GetStringAsync(baseUrl);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(response);

            string jsonData = null;

            var scriptNode = htmlDocument.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']");
            if (scriptNode != null)
            {
                jsonData = scriptNode.InnerText;
            }


            if (jsonData != null)
            {
                var jsonObject = JObject.Parse(jsonData);
                var listings = jsonObject["props"]?["pageProps"]?["listings"];

                if (listings != null)
                {
                    Console.WriteLine($"Retrieved {listings.Count()} listings from the JSON data."); // Log the number of listings

                    foreach (var listing in listings)
                    {
                        // Create a default property listing with only the known information
                        var title = listing["title"]?.ToString();
                        var priceText = listing["price"]?.ToString();
                        decimal price = 0;

                        if (!string.IsNullOrEmpty(priceText) && decimal.TryParse(priceText, out var parsedPrice))
                        {
                            price = parsedPrice;
                        }

                        var propertyListing = new PropertyListing
                        {
                            Title = title ?? "Unknown title",
                            Price = price,
                        };

                        var listingId = listing["id"]?.ToString();

                        if (listingId != null)
                        {
                            await Task.Delay(500); // Adding a delay of 0.5 second before making a new request

                            var detailUrl = baseUrl + listingId;
                            var detailResponse = await httpClient.GetAsync(detailUrl);

                            // If the detail page is found, try to extract more details to update the property listing
                            if (detailResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
                            {
                                var detailResponseContent = await detailResponse.Content.ReadAsStringAsync();
                                var detailHtmlDocument = new HtmlAgilityPack.HtmlDocument();
                                detailHtmlDocument.LoadHtml(detailResponseContent);

                                var detailScriptNode = detailHtmlDocument.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']");
                                if (detailScriptNode != null)
                                {
                                    var detailJsonData = detailScriptNode.InnerText;
                                    var detailJsonObject = JObject.Parse(detailJsonData);
                                    var detailListing = detailJsonObject["props"]?["pageProps"]?["listing"];

                                    // If detailed information is found, update the property listing with the detailed information
                                    if (detailListing != null)
                                    {
                                        var address = detailListing["address"];
                                        var addressStreet = address?["street"]?.ToString();
                                        var addressStreetNumber = address?["streetNumber"]?.ToString();
                                        var zipCode = address?["zipCode"]?.ToString();
                                        var addressCity = address?["city"]?.ToString();
                                        var additionalCosts = detailListing["additionalCosts"]?.ToString();
                                        var company = detailListing["company"]?.ToString();
                                        var firstName = detailListing["firstName"]?.ToString();
                                        var lastName = detailListing["lastName"]?.ToString();
                                        var phone = detailListing["phone"]?.ToString();
                                        var email = detailListing["email"]?.ToString();

                                        var imageUrlNode = detailHtmlDocument.DocumentNode.SelectSingleNode("//img");
                                        var imageUrl = imageUrlNode?.GetAttributeValue("src", "No image URL");

                                        string fullAddress = (addressStreet ?? "Unknown street") + " " + (addressStreetNumber ?? "Unknown number") + ", " + (zipCode ?? "Unknown zip code") + ", " + (addressCity ?? "Unknown city");
                                        string contact = (company ?? "Unknown company") + ", " + (firstName ?? "Unknown first name") + " " + (lastName ?? "Unknown last name") + ", " + (phone ?? "Unknown phone") + ", " + (email ?? "Unknown email");

                                        propertyListing.Address = fullAddress;
                                        propertyListing.AdditionalCosts = additionalCosts;
                                        propertyListing.Contact = contact;
                                        propertyListing.ImageUrl = imageUrl;
                                        propertyListing.FirstName = firstName;
                                        propertyListing.LastName = lastName;
                                        propertyListing.Email = email;
                                        propertyListing.Phone = phone;
                                        propertyListing.Street = addressStreet;
                                        propertyListing.StreetNumber = addressStreetNumber;
                                        propertyListing.ZipCode = zipCode;
                                        propertyListing.City = addressCity;
                                    }
                                }
                            }
                        }

                        // Add the property listing to the context (either the default listing or the updated listing with more details)
                        _context.PropertyListings.Add(propertyListing);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
