using KGSoft.TinyHttpClient.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using static KGSoft.TinyHttpClient.Enums;

namespace KGSoft.TinyHttpClient;

public class HeaderConfig
{
    /// <summary>
    /// The AuthHeader to use for this request
    /// </summary>
    public AuthenticationHeaderValue AuthHeader = null;
    /// <summary>
    /// The CustomHeaders to use for this request
    /// </summary>
    public Dictionary<string, string> CustomHeaders = null;
    /// <summary>
    /// The Logger to use for this request
    /// </summary>
    public ILogger Logger = null;
    /// <summary>
    /// The LogScope to use for this request
    /// </summary>
    public LogScope? LogScope = null;
    /// <summary>
    /// The HttpClient to use for this request
    /// </summary>
    public HttpClient HttpClient = null;
}
