using KGSoft.TinyHttpClient.Logging;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace KGSoft.TinyHttpClient
{
    /// <summary>
    /// The global static configuration for our Http requests
    /// </summary>
    public static class HttpConfig
    {
        public static string MediaTypeHeader = "application/json";
        public static AuthenticationHeaderValue DefaultAuthHeader = null;
        public static Dictionary<string, string> CustomHeaders;
        public static ILogger Logger;
    }
}
