using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace KGSoft.TinyHttpClient.Test
{
    public class TestBase
    {
        public const string ApiBase = "https://reqres.in/";
        public const string ApiUnauthorized = "https://httpstat.us/401";
        public static AuthenticationHeaderValue AuthHeader = new AuthenticationHeaderValue("Bearer", "XYZ");
        public static Dictionary<string, string> CustomHeaders = new Dictionary<string, string>() { { "CustomKey", "CustomValue" } };

        public HeaderConfig CfgAuthHeader = new HeaderConfig() { AuthHeader = AuthHeader };
        public HeaderConfig CfgCustomHeaders = new HeaderConfig() { CustomHeaders = CustomHeaders };
        public HeaderConfig CfgFull = new HeaderConfig() { AuthHeader = AuthHeader, CustomHeaders = CustomHeaders };

        public void AssertResponse(Response response)
        {
            Assert.IsTrue(response.IsSuccess);
        }

        public void AssertResponse401(Response response)
        {
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        public void AssertResponseResult<T>(Response<T> response)
        {
            AssertResponse(response);
            Assert.IsNotNull(response.Result);
        }
    }
}
