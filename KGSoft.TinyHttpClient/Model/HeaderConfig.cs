using System.Collections.Generic;
using System.Net.Http.Headers;

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
}
