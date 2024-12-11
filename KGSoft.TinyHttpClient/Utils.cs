using KGSoft.TinyHttpClient.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KGSoft.TinyHttpClient;

public static class Utils
{
    public static string BuildUrl(string baseUrl, IEnumerable<RequestParam> requestParams = null)
    {
        if (requestParams == null || !requestParams.Any())
            return baseUrl;

        var sb = new StringBuilder();
        sb.Append(baseUrl.EnsureDoesNotEndWith("/"));

        foreach (var p in requestParams)
            sb.AppendFormat("{0}{1}={2}",
                p == requestParams.First() ? "?" : "&",
                p.Key,
                p.Value);

        return sb.ToString();
    }

    public static HeaderConfig BuildHeaderConfig(Dictionary<string, string> headers)
    {
        if (headers.Count == 0)
            return null;
        else
        {
            var cfg = new HeaderConfig
            {
                CustomHeaders = new Dictionary<string, string>()
            };

            foreach (var h in headers)
                cfg.CustomHeaders.Add(h.Key, h.Value);

            return cfg;
        }
    }
}
