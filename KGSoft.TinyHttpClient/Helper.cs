using KGSoft.TinyHttpClient.Config;
using KGSoft.TinyHttpClient.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient
{
    public static class Helper
    {
        #region GET
        /// <summary>
        /// Create a GET request without expecting a type returned
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response> GetAsync(string url, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest(url, HttpMethod.Get, string.Empty, tkn);
        }

        /// <summary>
        /// Create a GET request expecting T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response<T>> GetAsync<T>(string url, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest<T>(url, HttpMethod.Get, string.Empty, tkn);
        }
        #endregion

        #region POST
        /// <summary>
        /// Create a POST request without expecting a type returned
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response> PostAsync(string url, string body, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest(url, HttpMethod.Post, body, tkn);
        }

        /// <summary>
        /// Create a POST request expecting T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response<T>> PostAsync<T>(string url, string body, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest<T>(url, HttpMethod.Post, body, tkn);
        }
        #endregion

        #region PUT
        /// <summary>
        /// Create a PUT request without expecting a type returned
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response> PutAsync(string url, string body, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest(url, HttpMethod.Put, body, tkn);
        }

        /// <summary>
        /// Create a PUT request expecting T in the response 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response<T>> PutAsync<T>(string url, string body, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest<T>(url, HttpMethod.Put, body, tkn);
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Create a DELETE request without expecting a type returned
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response> DeleteAsync(string url, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest(url, HttpMethod.Delete, string.Empty, tkn);
        }

        /// <summary>
        /// Create a DELETE request expecting T in the response
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        public static Task<Response<T>> DeleteAsync<T>(string url, CancellationToken tkn = default(CancellationToken))
        {
            return MakeHttpRequest<T>(url, HttpMethod.Delete, string.Empty, tkn);
        }
        #endregion

        #region UtilMethods
        /// <summary>
        /// A Generic version of MakeHttpRequest.
        /// Used when we are expecing and actual type to come as a response that we need to deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        private static async Task<Response<T>> MakeHttpRequest<T>(string url, HttpMethod method, string body = "", CancellationToken tkn = default(CancellationToken))
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            LogHelper.LogMessage($"[{method.Method}]: {url}");

            if (!string.IsNullOrEmpty(body))
                request.Content = CreateContent(body);

            var message = await HttpClientPool.Client.SendAsync(request, tkn);
            LogHelper.LogMessage($"[ResponseCode]: {message.StatusCode}");

            return await message.BuildResponse<T>();
        }

        /// <summary>
        /// An HttpRequest with a hollow response. Used for things like Http.Delete where we do not expect to deserialize anything
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        private static async Task<Response> MakeHttpRequest(string url, HttpMethod method, string body = "", CancellationToken tkn = default(CancellationToken))
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            LogHelper.LogMessage($"[{method.Method}]: {url}");

            if (!string.IsNullOrEmpty(body))
                request.Content = CreateContent(body);

            var message = await HttpClientPool.Client.SendAsync(request, tkn);
            LogHelper.LogMessage($"[ResponseCode]: {message.StatusCode}");

            return await message.BuildResponse();
        }

        /// <summary>
        /// Returns HttpContent in the configured media format
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private static HttpContent CreateContent(string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                var buffer = System.Text.Encoding.UTF8.GetBytes(body);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue(HttpConfig.MediaTypeHeader);
                return byteContent;
            }
            return new StringContent(string.Empty);
        }
        #endregion
    }
}
