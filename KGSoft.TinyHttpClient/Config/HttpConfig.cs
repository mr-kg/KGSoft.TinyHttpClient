using KGSoft.TinyHttpClient.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static KGSoft.TinyHttpClient.Enums;

namespace KGSoft.TinyHttpClient;

/// <summary>
/// The global static configuration for our Http requests
/// </summary>
public static class HttpConfig
{
    /// <summary>
    /// The MIME type we are expecting to use
    /// </summary>
    public static string MediaTypeHeader = Constants.ApplicationJson;

    /// <summary>
    /// Default Authorization header
    /// </summary>
    public static AuthenticationHeaderValue DefaultAuthHeader = null;

    /// <summary>
    /// Custom headers we need to add to our requests
    /// </summary>
    public static Dictionary<string, string> CustomHeaders = new();

    /// <summary>
    /// The scope of the logger
    /// </summary>
    public static LogScope LogScope = LogScope.OnlyFailedRequests;

    /// <summary>
    /// The Logger instance to be used by the entire context
    /// </summary>
    public static ILogger Logger;

    /// <summary>
    /// An action to invoke pre-request.
    /// Can be used for acquiring OAuth tokens and adding them as Custom Headers
    /// </summary>
    public static Action PreRequestAuthAction;

    /// <summary>
    /// A function to invoke pre-request.
    /// Can be used for acquiring OAuth tokens and adding them as Custom Headers
    /// </summary>
    public static Func<Task> PreRequestAuthAsyncFunc;

    /// <summary>
    /// An action to be performed when a 401 Unauthorized result is received.
    /// Can be used to trigger token refresh/authentication flow automatically
    /// </summary>
    public static Action UnauthorizedResultAction;

    /// <summary>
    /// A function to be performed when a 401 Unauthorized result is received.
    /// Can be used to trigger token refresh/authentication flow automatically
    /// </summary>
    public static Func<Task> UnauthorizedResultAsyncFunc;
}
