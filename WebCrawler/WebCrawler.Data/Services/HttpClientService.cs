using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Data.Interfaces;

namespace WebCrawler.Data.Services
{
    public class HttpClientService : IHttpClientService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<string> GetStringAsync(string url)
        {
            return await httpClient.GetStringAsync(url);
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await httpClient.GetAsync(url);
        }
    }
}
