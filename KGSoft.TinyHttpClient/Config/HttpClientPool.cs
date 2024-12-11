using System;
using System.Net.Http;

namespace KGSoft.TinyHttpClient.Config;

public static class HttpClientPool
{
    private static readonly Lazy<HttpClient> _lazyHttpClient = new(CreateHttpClient);

    public static HttpClient Client => _lazyHttpClient.Value;

    private static HttpClient CreateHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(HttpConfig.HttpClientPoolLifetimeMinutes), // Ensures DNS changes are respected
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(HttpConfig.HttpClientPoolIdleMinutes), // Closes idle connections after a timeout
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(HttpConfig.RequestTimeoutSeconds) // Example timeout
        };

        return client;
    }
}
