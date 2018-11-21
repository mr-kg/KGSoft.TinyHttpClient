using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient
{
    static class Extensions
    {
        /// <summary>
        /// Builds a response of T from the HttpResponseMessage
        /// </summary>
        /// <typeparam name="T">The type we're expecting from the HttpRequest</typeparam>
        /// <param name="message">The HttpResponseMessage we need to parse</param>
        /// <returns></returns>
        public static async Task<Response<T>> BuildResponse<T>(this HttpResponseMessage message)
        {
            var r = await message.BuildResponse();
            var response = r.Convert<T>();

            if (typeof(T) != typeof(string) && message.IsSuccessStatusCode)
                response.Result = JsonConvert.DeserializeObject<T>(response.Message);

            return response;
        }

        /// <summary>
        /// Builds a response from the HttpResponseMessage
        /// </summary>
        /// <param name="message">The HttpResponseMessage we need to parse</param>
        /// <returns></returns>
        public static async Task<Response> BuildResponse(this HttpResponseMessage message)
        {
            return new Response()
            {
                IsSuccess = message.IsSuccessStatusCode,
                Message = await message.Content.ReadAsStringAsync(),
                StatusCode = message.StatusCode
            };
        }

        /// <summary>
        /// Converts a Response to a Response<T>
        /// </summary>
        /// <typeparam name="T">The type we're expecting from the HttpRequest</typeparam>
        /// <param name="response">The Response object we want to convert to Response<T></param>
        /// <returns></returns>
        public static Response<T> Convert<T>(this Response response)
        {
            return new Response<T>()
            {
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                StatusCode = response.StatusCode
            };
        }
    }
}
