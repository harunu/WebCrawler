using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data.Interfaces;
using WebCrawler.Data.Models;

namespace WebCrawler.Data.Services
{
    public class HtmlParserService : IHtmlParserService
    {
        public string GetJsonDataFromHtml(string htmlContent)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);
            var scriptNode = htmlDocument.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']");
            return scriptNode?.InnerText;
        }

        public JToken GetPropertyListingsFromJsonData(string jsonData)
        {
            var jsonObject = JObject.Parse(jsonData);
            return jsonObject["props"]?["pageProps"]?["listings"];
        }

        public JObject GetDetailJsonObject(string detailResponseContent)
        {
            var detailHtmlDocument = new HtmlDocument();
            detailHtmlDocument.LoadHtml(detailResponseContent);

            var detailScriptNode = detailHtmlDocument.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']");
            if (detailScriptNode != null)
            {
                var detailJsonData = detailScriptNode.InnerText;
                return JObject.Parse(detailJsonData);
            }

            return null;
        }

        public string GetImageUrl(HtmlDocument detailHtmlDocument)
        {
            var imageUrlNode = detailHtmlDocument.DocumentNode.SelectSingleNode("//img");
            return imageUrlNode?.GetAttributeValue("src", "No image URL");
        }

        public PropertyListing CreatePropertyListingFromJson(JToken listing)
        {
            var title = listing["title"]?.ToString();
            var priceText = listing["price"]?.ToString();
            decimal price = 0;

            if (!string.IsNullOrEmpty(priceText) && decimal.TryParse(priceText, out var parsedPrice))
            {
                price = parsedPrice;
            }

            return new PropertyListing
            {
                Title = title ?? "Unknown title",
                Price = price
            };
        }

        public void UpdatePropertyListingWithDetailInfo(JObject detailJsonObject, PropertyListing propertyListing, string imageUrl)
        {
            var detailListing = detailJsonObject["props"]?["pageProps"]?["listing"];

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
