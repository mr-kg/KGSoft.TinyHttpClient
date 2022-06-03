using KGSoft.TinyHttpClient.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient.Test
{
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
    }
}
