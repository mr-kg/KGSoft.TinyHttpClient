using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace KGSoft.TinyHttpClient.Test;

public class TestBase
{
    public const string ApiBase = "https://reqres.in/";
    public const string ApiUnauthorized = "https://httpstat.us/401";
    internal static readonly AuthenticationHeaderValue AuthHeader = new("Bearer", "XYZ");
    internal static readonly Dictionary<string, string> CustomHeaders = new() { { "CustomKey", "CustomValue" } };

    public HeaderConfig CfgAuthHeader = new() { AuthHeader = AuthHeader };
    public HeaderConfig CfgCustomHeaders = new() { CustomHeaders = CustomHeaders };
    public HeaderConfig CfgFull = new() { AuthHeader = AuthHeader, CustomHeaders = CustomHeaders };

    public static void AssertResponse(Response response)
    {
        Assert.IsTrue(response.IsSuccess);
    }

    public static void AssertResponse401(Response response)
    {
        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
    }

    public static void AssertResponseResult<T>(Response<T> response)
    {
        AssertResponse(response);
        Assert.IsNotNull(response.Result);
    }
}
