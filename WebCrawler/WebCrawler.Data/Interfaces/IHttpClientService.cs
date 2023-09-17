namespace WebCrawler.Data.Interfaces
{
    public interface IHttpClientService
    {
        Task<string> GetStringAsync(string url);
        Task<HttpResponseMessage> GetAsync(string url);
    }
}
