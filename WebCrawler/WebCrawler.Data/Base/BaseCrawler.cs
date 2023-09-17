using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Data.Base
{
    public abstract class BaseCrawler
    {
        private readonly HttpClient _httpClient;
        private readonly ILog _log;

        protected BaseCrawler(HttpClient httpClient, ILog log)
        {
            _httpClient = httpClient;
            _log = log;
        }

        protected async Task<string> FetchContentAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to fetch content from {url}.", ex);
                throw;
            }
        }

        protected HtmlDocument ParseHtmlContent(string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        public abstract Task ExtractDataAndStoreAsync(string url);
    }

    public interface ILog
    {
        void Error(string message, Exception exception);
    }
}
