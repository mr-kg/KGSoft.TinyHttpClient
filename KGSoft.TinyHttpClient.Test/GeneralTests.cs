using KGSoft.TinyHttpClient.Model;
using KGSoft.TinyHttpClient.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient.Test;

[TestClass]
public class GeneralTests : TestBase
{
    [TestMethod]
    public async Task Test_Content_And_Message_Equal()
    {
        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/users/2")
            .MakeRequestAsync<TestUser>();

        var contentString = Encoding.Default.GetString(response.Content);
        Assert.IsNotNull(contentString);
        Assert.AreEqual(contentString, response.Message);
    }

    [TestMethod]
    public void Test_Extension_EnsureDoesNotEndWith_SingleChar()
    {
        var urlWithEnding = "https://google.com/";
        var urlWithoutEnding = "https://google.com";

        Assert.AreEqual(urlWithoutEnding, urlWithEnding.EnsureDoesNotEndWith("/"));
    }

    [TestMethod]
    public void Test_Extension_EnsureDoesNotEndWith_MultiChar()
    {
        var urlWithEnding = "https://google.com/";
        var urlWithoutEnding = "https://google";

        Assert.AreEqual(urlWithoutEnding, urlWithEnding.EnsureDoesNotEndWith(".com/"));
    }

    [TestMethod]
    public void Test_Url_Creation_No_Params()
    {
        var url = "https://google.com";
        Assert.AreEqual(url, Utils.BuildUrl(url));
    }

    [TestMethod]
    public void Test_Url_Creation_One_Param()
    {
        var baseUrl = "https://google.com/";
        var urlExpected = "https://google.com?woo=hoo";
        var param = new List<RequestParam>
        {
            new RequestParam("woo", "hoo", Enums.RequestParamType.QueryString)
        };

        var r = Utils.BuildUrl(baseUrl, param);

        Assert.AreEqual(urlExpected, r);
    }

    [TestMethod]
    public void Test_Url_Creation_Two_Params()
    {
        var baseUrl = "https://google.com/";
        var urlExpected = "https://google.com?woo=hoo&foo=bar";
        var param = new List<RequestParam>
        {
            new("woo", "hoo", Enums.RequestParamType.QueryString),
            new("foo", "bar", Enums.RequestParamType.QueryString)
        };

        var r = Utils.BuildUrl(baseUrl, param);

        Assert.AreEqual(urlExpected, r);
    }

    [TestMethod]
    public void Test_Url_Creation_Existing_Query()
    {
        var baseUrl = "https://google.com/search?existing=true";
        var urlExpected = "https://google.com/search?existing=true&woo=hoo";
        var param = new List<RequestParam>
        {
            new("woo", "hoo", Enums.RequestParamType.QueryString)
        };

        var r = Utils.BuildUrl(baseUrl, param);

        Assert.AreEqual(urlExpected, r);
    }

    [TestMethod]
    public void Test_Url_Creation_Encodes_Params()
    {
        var baseUrl = "https://google.com/search";
        var urlExpected = "https://google.com/search?hello+world=a%26b%3Dc";
        var param = new List<RequestParam>
        {
            new("hello world", "a&b=c", Enums.RequestParamType.QueryString)
        };

        var r = Utils.BuildUrl(baseUrl, param);

        Assert.AreEqual(urlExpected, r);
    }

    [TestMethod]
    public void Test_Url_Creation_Preserves_Path_Trailing_Slash()
    {
        var baseUrl = "https://google.com/api/users/";
        var urlExpected = "https://google.com/api/users/?woo=hoo";
        var param = new List<RequestParam>
        {
            new("woo", "hoo", Enums.RequestParamType.QueryString)
        };

        var r = Utils.BuildUrl(baseUrl, param);

        Assert.AreEqual(urlExpected, r);
    }

    [TestMethod]
    public void Test_Url_Creation_Preserves_Fragment()
    {
        var baseUrl = "https://google.com/search#results";
        var urlExpected = "https://google.com/search?woo=hoo#results";
        var param = new List<RequestParam>
        {
            new("woo", "hoo", Enums.RequestParamType.QueryString)
        };

        var r = Utils.BuildUrl(baseUrl, param);

        Assert.AreEqual(urlExpected, r);
    }

    [TestMethod]
    public async Task Test_Response_String_Result()
    {
        using var message = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("plain response")
        };

        var response = await message.BuildResponse<string>();

        Assert.AreEqual("plain response", response.Result);
    }
}
