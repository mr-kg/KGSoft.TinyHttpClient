using System.Net.Http;

namespace KGSoft.TinyHttpClient.Config;

static class HttpClientPool
{
    static HttpClient _httpClient;

    public static HttpClient Client
    {
        get
        {
            return _httpClient ??= new HttpClient();
        }
    }
}
