using KGSoft.TinyHttpClient.Model;
using KGSoft.TinyHttpClient.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;


namespace KGSoft.TinyHttpClient.Test;

[TestClass]
public class HttpRequestBuilderTests : TestBase
{
    [TestMethod]
    public async Task Test_GET()
    {
        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/users/2")
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_GET_Result()
    {
        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/users/2")
            .MakeRequestAsync<TestUser>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_GET_MissingUri()
    {
        try
        {
            await new HttpRequestBuilder()
                .Get(string.Empty)
                .MakeRequestAsync();

            Assert.Fail($"{nameof(MissingUriException)} was not thrown.");
        }
        catch (MissingUriException)
        {
        }
    }

    [TestMethod]
    public async Task Test_GET_MismatchedType()
    {
        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/users/2")
            .MakeRequestAsync<MismatchedType>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_PUT()
    {
        var response = await new HttpRequestBuilder()
            .Put($"{ApiBase}api/users/2")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_PUT_Result()
    {
        var response = await new HttpRequestBuilder()
            .Put($"{ApiBase}api/users/2")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync<TestUser>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_POST()
    {
        var response = await new HttpRequestBuilder()
            .Post($"{ApiBase}api/users")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_POST_Result()
    {
        var response = await new HttpRequestBuilder()
            .Post($"{ApiBase}api/users")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync<TestUser>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_DELETE()
    {
        var response = await new HttpRequestBuilder()
            .Delete($"{ApiBase}api/users")
            .MakeRequestAsync();

        AssertResponse(response);
    }
}
