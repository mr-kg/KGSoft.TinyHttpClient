using System;
using System.Threading.Tasks;
using KGSoft.TinyHttpClient.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace KGSoft.TinyHttpClient.Tests
{
    [TestClass]
    public class HttpRequestTests
    {
        const string ApiBase = "https://reqres.in/";

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
    }
}
