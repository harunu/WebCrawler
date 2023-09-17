using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data.Models;

namespace WebCrawler.Data.Interfaces
{
    public interface IHtmlParserService
    {
        string GetJsonDataFromHtml(string htmlDocument);
        JToken GetPropertyListingsFromJsonData(string jsonData);
        JObject GetDetailJsonObject(string detailResponseContent);
        string GetImageUrl(HtmlDocument detailHtmlDocument);
        PropertyListing CreatePropertyListingFromJson(JToken listing);
        void UpdatePropertyListingWithDetailInfo(JObject detailJsonObject, PropertyListing propertyListing, string imageUrl);
    }

}
