using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient
{
    public static class Extensions
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

            if (typeof(T) != typeof(string))
            {
                if (HttpConfig.MediaTypeHeader == Constants.ApplicationJson)
                    response.Result = response.Message.TryDeserializeFromJson<T>();
                else
                    Logging.LogHelper.LogMessage($"Unable to auto-deserialize MIME type: {HttpConfig.MediaTypeHeader} into Response.Result<T>. Please manually deserialize Response.Message instead.");

                // TODO: add deserializers
            }

            return response;
        }

        /// <summary>
        /// Builds a response from the HttpResponseMessage
        /// </summary>
        /// <param name="message">The HttpResponseMessage we need to parse</param>
        /// <returns></returns>
        public static async Task<Response> BuildResponse(this HttpResponseMessage message)
            => new Response()
            {
                IsSuccess = message.IsSuccessStatusCode,
                Content = await message.Content.ReadAsByteArrayAsync(),
                Message = await message.Content.ReadAsStringAsync(),
                StatusCode = message.StatusCode
            };

        /// <summary>
        /// Converts a Response to a Response<T>
        /// </summary>
        /// <typeparam name="T">The type we're expecting from the HttpRequest</typeparam>
        /// <param name="response">The Response object we want to convert to Response<T></param>
        /// <returns></returns>
        public static Response<T> Convert<T>(this Response response)
            => new Response<T>()
            {
                IsSuccess = response.IsSuccess,
                Content = response.Content,
                Message = response.Message,
                StatusCode = response.StatusCode
            };

        public static string EnsureDoesNotEndWith(this string value, string c)
            => value.EndsWith(c) ? value.Substring(0, value.Length - c.Length) : value;

        public static string TrimAndLower(this string value)
            => value.Trim().ToLower();

        public static T TryDeserializeFromJson<T>(this string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                // Could not deserialize
                return default;
            }
        }
    }
}
