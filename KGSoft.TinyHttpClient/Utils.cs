using KGSoft.TinyHttpClient.Model;
using KGSoft.TinyHttpClient.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using static KGSoft.TinyHttpClient.Enums;

namespace KGSoft.TinyHttpClient;

public static class Utils
{
    public static string BuildUrl(string baseUrl, IEnumerable<RequestParam> requestParams = null)
    {
        var parameters = requestParams?.ToList();
        if (parameters == null || !parameters.Any())
            return baseUrl;

        var sb = new StringBuilder();
        var fragment = string.Empty;
        var fragmentIndex = baseUrl.IndexOf('#');
        if (fragmentIndex >= 0)
        {
            fragment = baseUrl[fragmentIndex..];
            baseUrl = baseUrl[..fragmentIndex];
        }

        sb.Append(NormalizeBaseUrlForQueryString(baseUrl));
        sb.Append(baseUrl.Contains('?') ? "&" : "?");
        sb.Append(string.Join("&", parameters.Select(p =>
            $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}")));
        sb.Append(fragment);

        return sb.ToString();
    }

    public static HeaderConfig BuildHeaderConfig(
        Dictionary<string, string> headers,
        AuthenticationHeaderValue authHeader = null,
        ILogger logger = null,
        LogScope? logScope = null,
        HttpClient httpClient = null)
    {
        if (headers.Count == 0 && authHeader == null && logger == null && logScope == null && httpClient == null)
            return null;
        else
        {
            var cfg = new HeaderConfig
            {
                AuthHeader = authHeader,
                Logger = logger,
                LogScope = logScope,
                HttpClient = httpClient,
                CustomHeaders = new Dictionary<string, string>()
            };

            foreach (var h in headers)
                cfg.CustomHeaders.Add(h.Key, h.Value);

            return cfg;
        }
    }

    private static string NormalizeBaseUrlForQueryString(string baseUrl)
    {
        if (Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
            && uri.AbsolutePath == "/"
            && string.IsNullOrEmpty(uri.Query))
            return baseUrl.EnsureDoesNotEndWith("/");

        return baseUrl;
    }
}
