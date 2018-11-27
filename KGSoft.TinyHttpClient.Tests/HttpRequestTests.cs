using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using KGSoft.TinyHttpClient.Tests.Model;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace KGSoft.TinyHttpClient.Tests
{
    [TestClass]
    public class HttpRequestTests
    {
        const string ApiBase = "https://reqres.in/";
        static AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", "XYZ");
        static Dictionary<string, string> customHeaders = new Dictionary<string, string>() { { "CustomKey", "CustomValue" } };

        HeaderConfig cfgAuthHeader = new HeaderConfig() { AuthHeader = authHeader };
        HeaderConfig cfgCustomHeaders = new HeaderConfig() { CustomHeaders = customHeaders };
        HeaderConfig cfgFull = new HeaderConfig() { AuthHeader = authHeader, CustomHeaders = customHeaders };

        private void AssertResponse(Response response)
        {
            Assert.IsTrue(response.IsSuccess);
        }

        private void AssertResponseResult<T>(Response<T> response)
        {
            AssertResponse(response);
            Assert.IsNotNull(response.Result);
        }

        [TestMethod]
        public async Task Test_GET()
        {
            AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2"));
        }

        [TestMethod]
        public async Task Test_GET_GlobalHeaders()
        {
            HttpConfig.CustomHeaders = customHeaders;
            HttpConfig.DefaultAuthHeader = authHeader;
            AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2"));
            HttpConfig.CustomHeaders.Clear();
            HttpConfig.DefaultAuthHeader = null;
        }

        [TestMethod]
        public async Task Test_GET_AuthHeader()
        {
            AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2", config: cfgAuthHeader ));
        }

        [TestMethod]
        public async Task Test_GET_CustomHeader()
        {
            AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2", config: cfgCustomHeaders));
        }

        [TestMethod]
        public async Task Test_GET_CustomHeadersAll()
        {
            AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2", config: cfgFull));
        }

        [TestMethod]
        public async Task Test_GET_Result()
        {
            AssertResponseResult(await Helper.GetAsync<TestUser>(ApiBase + "api/users/2"));
        }

        [TestMethod]
        public async Task Test_PUT()
        {
            AssertResponse(await Helper.PutAsync(ApiBase + "api/users/2", JsonConvert.SerializeObject(new data() { first_name = "Chewbacca" })));
        }

        [TestMethod]
        public async Task Test_PUT_Result()
        {
            AssertResponseResult(await Helper.PutAsync<TestUser>(ApiBase + "api/users/2", 
                JsonConvert.SerializeObject(new data() { first_name = "Chewbacca" })));
        }

        [TestMethod]
        public async Task Test_POST()
        {
            AssertResponse(await Helper.PostAsync(ApiBase + "api/users", JsonConvert.SerializeObject(new data() { first_name = "Chewbacca" })));
        }

        [TestMethod]
        public async Task Test_POST_Result()
        {
            AssertResponseResult(await Helper.PostAsync<TestUser>(ApiBase + "api/users",
                JsonConvert.SerializeObject(new data() { first_name = "Chewbacca" })));
        }

        [TestMethod]
        public async Task Test_DELETE()
        {
            AssertResponse(await Helper.DeleteAsync(ApiBase + "api/users/2"));
        }

        [TestMethod]
        public async Task Test_404()
        {
            var response = await Helper.GetAsync(ApiBase + "api/unknown/23");
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task Test_400()
        {
            var response = await Helper.PostAsync(ApiBase + "/api/register", "{ \"email\": \"sydney@fife\" }");
            Assert.IsFalse(response.IsSuccess);
        }

        /*
         * Uncomment and run the following tests if your are using AzureAD authentication, and need to acquire/refresh your
         * token on every HttpRequest.
         * 
         * This is a very good use case for the PreAuthAsyncFunc
         */

        //[TestMethod]
        //public async Task Test_PreAuth()
        //{
        //    HttpConfig.PreRequestAuthAsyncFunc = GetAuthBearerToken;
        //    var response = await Helper.GetAsync("some protected API");
        //}

        //private async Task GetAuthBearerToken()
        //{
        //    var authority = "";
        //    var resource = "";
        //    var clientId = "";
        //    var redirectUri = "";
        //    var extraQueryParams = "";

        //    var authContext = new AuthenticationContext(authority);
        //    PlatformParameters platform = new PlatformParameters(PromptBehavior.Auto);

        //    // Call to the ADAL to get a token
        //    var result = await authContext.AcquireTokenAsync(
        //         resource,
        //         clientId,
        //         new Uri(redirectUri),
        //         platform,
        //         UserIdentifier.AnyUser,
        //         extraQueryParams);

        //    if (result != null) // We get a token, and we set it as the AuthHeader for our HttpClient in the global config
        //        HttpConfig.DefaultAuthHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
        //}
    }
}
