using System.Net.Http;
using System.Net.Http.Headers;

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

                    if (HttpConfig.DefaultAuthHeader != null)
                        _httpClient.DefaultRequestHeaders.Authorization = HttpConfig.DefaultAuthHeader;

                    if (HttpConfig.CustomHeaders != null)
                        foreach (var kvp in HttpConfig.CustomHeaders)
                            _httpClient.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);

                    _httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(HttpConfig.MediaTypeHeader));
                }
                return _httpClient;
            }
        }

        /// <summary>
        /// Since we're re-using a single HttpClient, this method allows us to reset our HttpClient in case we need to change any config
        /// after HttpClient has been instatiated for the first time
        /// </summary>
        public static void Reset()
        {
            _httpClient = null;
        }
    }
}
