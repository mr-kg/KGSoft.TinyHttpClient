using System.Threading.Tasks;
using KGSoft.TinyHttpClient.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace KGSoft.TinyHttpClient.Test;

[TestClass]
public class HelperTests : TestBase
{
    [TestMethod]
    public async Task Test_GET()
    {
        AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2"));
    }

    [TestMethod]
    public async Task Test_GET_GlobalHeaders()
    {
        HttpConfig.CustomHeaders = CustomHeaders;
        HttpConfig.DefaultAuthHeader = AuthHeader;
        AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2"));
        HttpConfig.CustomHeaders.Clear();
        HttpConfig.DefaultAuthHeader = null;
    }

    [TestMethod]
    public async Task Test_GET_AuthHeader()
    {
        AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2", config: CfgAuthHeader ));
    }

    [TestMethod]
    public async Task Test_GET_CustomHeader()
    {
        AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2", config: CfgCustomHeaders));
    }

    [TestMethod]
    public async Task Test_GET_CustomHeadersAll()
    {
        AssertResponse(await Helper.GetAsync(ApiBase + "api/users/2", config: CfgFull));
    }

    [TestMethod]
    public async Task Test_GET_Result()
    {
        AssertResponseResult(await Helper.GetAsync<TestUser>(ApiBase + "api/users/2"));
    }

    [TestMethod]
    public async Task Test_PUT()
    {
        AssertResponse(await Helper.PutAsync(ApiBase + "api/users/2", JsonConvert.SerializeObject(new Data() { FirstName = "Chewbacca" })));
    }

    [TestMethod]
    public async Task Test_PUT_Result()
    {
        AssertResponseResult(await Helper.PutAsync<TestUser>(ApiBase + "api/users/2", 
            JsonConvert.SerializeObject(new Data() { FirstName = "Chewbacca" })));
    }

    [TestMethod]
    public async Task Test_POST()
    {
        AssertResponse(await Helper.PostAsync(ApiBase + "api/users", JsonConvert.SerializeObject(new Data() { FirstName = "Chewbacca" })));
    }

    [TestMethod]
    public async Task Test_POST_Result()
    {
        AssertResponseResult(await Helper.PostAsync<TestUser>(ApiBase + "api/users",
            JsonConvert.SerializeObject(new Data() { FirstName = "Chewbacca" })));
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

    [TestMethod]
    public async Task Test_401Action()
    {
        bool actionFlag = false;
        HttpConfig.UnauthorizedResultAction = () => { actionFlag = true; };
        AssertResponse401(await Helper.GetAsync(ApiUnauthorized));
        Assert.IsTrue(actionFlag);

        HttpConfig.UnauthorizedResultAction = null;
    }

    [TestMethod]
    public async Task Test_401Func()
    {
        bool actionFlag = false;
        HttpConfig.UnauthorizedResultAsyncFunc = () => { actionFlag = true; return Task.CompletedTask; };
        AssertResponse401(await Helper.GetAsync(ApiUnauthorized));
        Assert.IsTrue(actionFlag);

        HttpConfig.UnauthorizedResultAsyncFunc = null;
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
