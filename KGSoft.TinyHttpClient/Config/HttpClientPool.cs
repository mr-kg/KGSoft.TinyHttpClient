using System;
using System.Net.Http;

namespace KGSoft.TinyHttpClient.Config;

public static class HttpClientPool
{
    private static readonly object _lock = new();
    private static HttpClient _client;
    private static int _clientLifetimeMinutes;
    private static int _clientIdleMinutes;
    private static int _requestTimeoutSeconds;

    public static HttpClient Client
    {
        get
        {
            lock (_lock)
            {
                if (_client == null || ConfigHasChanged())
                {
                    _client = CreateHttpClient();
                }

                return _client;
            }
        }
    }

    private static bool ConfigHasChanged()
        => _clientLifetimeMinutes != HttpConfig.HttpClientPoolLifetimeMinutes
            || _clientIdleMinutes != HttpConfig.HttpClientPoolIdleMinutes
            || _requestTimeoutSeconds != HttpConfig.RequestTimeoutSeconds;

    private static HttpClient CreateHttpClient()
    {
        _clientLifetimeMinutes = HttpConfig.HttpClientPoolLifetimeMinutes;
        _clientIdleMinutes = HttpConfig.HttpClientPoolIdleMinutes;
        _requestTimeoutSeconds = HttpConfig.RequestTimeoutSeconds;

        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(_clientLifetimeMinutes), // Ensures DNS changes are respected
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(_clientIdleMinutes), // Closes idle connections after a timeout
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(_requestTimeoutSeconds)
        };

        return client;
    }
}
