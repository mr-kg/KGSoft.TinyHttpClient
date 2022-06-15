using KGSoft.TinyHttpClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient
{
    public class HttpRequestBuilder
    {
        private string _uri;
        private readonly Dictionary<string, string> _headers;
        private readonly List<RequestParam> _requestParams;
        private HttpMethod _method;
        private object _body;
        private CancellationToken _cancellationToken;

        /// <summary>
        /// Default ctor for the Request Builder
        /// </summary>
        public HttpRequestBuilder()
        {
            _headers = new Dictionary<string, string>();
            _requestParams = new List<RequestParam>();
        }

        /// <summary>
        /// Signifies the intent for a GET request
        /// </summary>
        /// <param name="uri">The URI to be targeted</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder Get(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Get;
            return this;
        }

        /// <summary>
        /// Signifies the intent for a POST request
        /// </summary>
        /// <param name="uri">The URI to be targeted</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder Post(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Post;
            return this;
        }

        /// <summary>
        /// Signifies the intent for a PUT request
        /// </summary>
        /// <param name="uri">The URI to be targeted</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder Put(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Put;
            return this;
        }

        /// <summary>
        /// Signifies the intent for a DELETE request
        /// </summary>
        /// <param name="uri">The URI to be targeted</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder Delete(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Delete;
            return this;
        }

        /// <summary>
        /// Adds a parameter to be added to the query string
        /// </summary>
        /// <param name="name">Key for the parameter</param>
        /// <param name="value">Value for the parameter</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder AddQueryParam(string name, string value)
        {
            if (!_requestParams.Any(x => x.Type == Enums.RequestParamType.QueryString && x.Key.TrimAndLower() == name.TrimAndLower()))
                _requestParams.Add(new RequestParam(name, value, Enums.RequestParamType.QueryString));
            return this;
        }

        /// <summary>
        /// Adds a range of query string parameters
        /// </summary>
        /// <param name="keyValuePairs">The dictionaly of parameters to be added</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder AddQueryParams(Dictionary<string, string> keyValuePairs)
        {
            var builder = this;
            foreach (var kvp in keyValuePairs)
                builder = AddQueryParam(kvp.Key, kvp.Value);
            return builder;
        }

        /// <summary>
        /// TODO: Implement form encoded values
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        //public HttpRequestBuilder AddFormParam(string name, string value)
        //{
        //    if (!_requestParams.Any(x => x.Type == Enums.RequestParamType.FormEncoded && x.Key.TrimAndLower() == name.TrimAndLower()))
        //        _requestParams.Add(new RequestParam(name, value, Enums.RequestParamType.FormEncoded));
        //    return this;
        //}


        /// <summary>
        /// Adds cancellation token to the request
        /// </summary>
        /// <param name="CancellationToken">The CancellationToken to be added</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder AddCancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
            return this;
        }

        /// <summary>
        /// Adds a header with the Key of Authorization
        /// </summary>
        /// <param name="value">The value of the Authorization header. Eg 'Bearer xyz...'</param>
        /// <returns>HttpRequestBuilder</returns>
        public HttpRequestBuilder AddAuthorizationHeader(string value)
        {
            _headers.Add("Authorization", value);
            return this;
        }

        /// <summary>
        /// Adds a header to the request
        /// </summary>
        /// <param name="name">The key for the header</param>
        /// <param name="value">The value for the header</param>
        /// <returns></returns>
        public HttpRequestBuilder AddHeader(string name, string value)
        {
            _headers.Add(name, value);
            return this;
        }

        /// <summary>
        /// Adds a body object to the request
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public HttpRequestBuilder AddBody(object body)
        {
            _body = body;
            return this;
        }

        /// <summary>
        /// Makes the Http Request, WITHOUT a return type expected
        /// </summary>
        /// <returns>Response</returns>
        public Task<Response> MakeRequestAsync()
        {
            Validate();
            
            return Helper.MakeHttpRequest(Utils.BuildUrl(_uri, _requestParams), 
                _method, 
                _body != null ? JsonConvert.SerializeObject(_body) : string.Empty, 
                _cancellationToken,
                Utils.BuildHeaderConfig(_headers));
        }

        /// <summary>
        /// Makes the Http Request, WITH a return type expected
        /// </summary>
        /// <returns>Response<typeparamref name="T"/></returns>
        public Task<Response<T>> MakeRequestAsync<T>()
        {
            Validate();

            return Helper.MakeHttpRequest<T>(Utils.BuildUrl(_uri, _requestParams), 
                _method, 
                _body != null ? JsonConvert.SerializeObject(_body) : string.Empty, 
                _cancellationToken, 
                Utils.BuildHeaderConfig(_headers));
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_uri))
                throw new MissingUriException();
            if (_method == null)
                throw new MissingHttpMethodException();
        }
    }
}
