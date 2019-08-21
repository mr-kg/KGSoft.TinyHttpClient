using System.Net.Http;

namespace KGSoft.TinyHttpClient.Config
{
    static class HttpClientPool
    {
        static HttpClient _httpClient;

        public static HttpClient Client
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                return _httpClient;
            }
        }
    }
}
