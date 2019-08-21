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
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response> GetAsync(string url, CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest(url, HttpMethod.Get, string.Empty, tkn, config);

        /// <summary>
        /// Create a GET request expecting T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response<T>> GetAsync<T>(string url, CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest<T>(url, HttpMethod.Get, string.Empty, tkn, config);
        #endregion

        #region POST
        /// <summary>
        /// Create a POST request without expecting a type returned
        /// </summary>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="body">The body we are sending with the request</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response> PostAsync(string url, string body, CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest(url, HttpMethod.Post, body, tkn, config);

        /// <summary>
        /// Create a POST request expecting T in the response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="body">The body we are sending with the request</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response<T>> PostAsync<T>(string url, string body, CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest<T>(url, HttpMethod.Post, body, tkn, config);
        #endregion

        #region PUT
        /// <summary>
        /// Create a PUT request without expecting a type returned
        /// </summary>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="body">The body we are sending with the request</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response> PutAsync(string url, string body, CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest(url, HttpMethod.Put, body, tkn, config);

        /// <summary>
        /// Create a PUT request expecting T in the response 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="body">The body we are sending with the request</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response<T>> PutAsync<T>(string url, string body, CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest<T>(url, HttpMethod.Put, body, tkn, config);
        #endregion

        #region DELETE
        /// <summary>
        /// Create a DELETE request without expecting a type returned
        /// </summary>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="body">The body we are sending with the request</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response> DeleteAsync(string url, string body = "", CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest(url, HttpMethod.Delete, body, tkn, config);

        /// <summary>
        /// Create a DELETE request expecting T in the response
        /// </summary>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="body">The body we are sending with the request</param>
        /// <param name="tkn">The cancellation token we want to use with this request</param>
        /// <param name="config">Configuration unique to this request. Used to override what is defined in HttpConfig</param>
        /// <returns></returns>
        public static Task<Response<T>> DeleteAsync<T>(string url, string body = "", CancellationToken tkn = default(CancellationToken), HeaderConfig config = default(HeaderConfig))
            => MakeHttpRequest<T>(url, HttpMethod.Delete, body, tkn, config);
        #endregion

        #region UtilMethods
        /// <summary>
        /// A Generic version of MakeHttpRequest.
        /// Used when we are expecing and actual type to come as a response that we need to deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        private static async Task<Response<T>> MakeHttpRequest<T>(string url, HttpMethod method, string body = "", CancellationToken tkn = default(CancellationToken), HeaderConfig cfg = default(HeaderConfig))
        {
            var message = await GetResponseMessage(url, method, body, tkn, cfg);
            var response = await message.BuildResponse<T>();

            HandleLogging(response);

            return response;
        }

        /// <summary>
        /// An HttpRequest with a hollow response. Used for things like Http.Delete where we do not expect to deserialize anything
        /// </summary>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        private static async Task<Response> MakeHttpRequest(string url, HttpMethod method, string body = "", CancellationToken tkn = default(CancellationToken), HeaderConfig cfg = default(HeaderConfig))
        {
            var message = await GetResponseMessage(url, method, body, tkn, cfg);
            var response = await message.BuildResponse();

            HandleLogging(response);

            return response;
        }

        /// <summary>
        /// Evaluate whether to run any actions upon receiving a 401 Unathorized response, if any actions are sprcified in the HttpConfig
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private static async Task HandlePostRequestUnauthorizedActions(System.Net.HttpStatusCode statusCode)
        {
            if (statusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HttpConfig.UnauthorizedResultAction?.Invoke();
                if (HttpConfig.UnauthorizedResultAsyncFunc != null)
                    await HttpConfig.UnauthorizedResultAsyncFunc();
            }
        }

        /// <summary>
        /// Handles logging based on defined LogScope
        /// </summary>
        /// <param name="response"></param>
        /// <param name="message"></param>
        private static void HandleLogging(Response response)
        {
            if (HttpConfig.LogScope == Enums.LogScope.OnlyFailedRequests && !response.IsSuccess)
                Log(response);
            else if (HttpConfig.LogScope == Enums.LogScope.AllRequests)
                Log(response);

            void Log(Response r)
            {
                LogHelper.LogMessage(string.Format("{0}[ResponseCode]: {1} - {2}",
                        response.IsSuccess ? string.Empty : "FAILED HTTP REQUEST - ",
                        response.StatusCode,
                        response.Message));
            }
        }

        /// <summary>
        /// Create and send an HttpRequestMessage
        /// </summary>
        /// <param name="url">The URL we want to make the request to</param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="tkn"></param>
        /// <returns></returns>
        private static async Task<HttpResponseMessage> GetResponseMessage(string url, HttpMethod method, string body = "", CancellationToken tkn = default(CancellationToken), HeaderConfig cfg = default(HeaderConfig))
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpConfig.MediaTypeHeader));

            LogHelper.LogMessage($"[{method.Method}]: {url}");

            // Prepare the headers as per the config passed through
            if (cfg?.AuthHeader == null && HttpConfig.DefaultAuthHeader != null)
                request.Headers.Authorization = HttpConfig.DefaultAuthHeader;
            else if (cfg?.AuthHeader != null)
                request.Headers.Authorization = cfg.AuthHeader;

            // Prepare the custom headers as per the config passed through
            if (cfg?.CustomHeaders == null && HttpConfig.CustomHeaders != null)
                foreach (var kvp in HttpConfig.CustomHeaders)
                    request.Headers.Add(kvp.Key, kvp.Value);
            else if (cfg?.CustomHeaders != null)
                foreach (var kvp in cfg.CustomHeaders)
                    request.Headers.Add(kvp.Key, kvp.Value);

            // Prepare the body
            if (!string.IsNullOrEmpty(body))
                request.Content = CreateContent(body);
            
            // Fire any pre-auth delegates before sending the request
            HttpConfig.PreRequestAuthAction?.Invoke();
            if (HttpConfig.PreRequestAuthAsyncFunc != null)
                await HttpConfig.PreRequestAuthAsyncFunc();

            var message = await HttpClientPool.Client.SendAsync(request, tkn);

            // If we have any 401 actions defined, evaluate the result and fire them
            await HandlePostRequestUnauthorizedActions(message.StatusCode);

            return message;
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
