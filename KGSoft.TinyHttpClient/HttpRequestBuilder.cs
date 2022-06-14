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

        public HttpRequestBuilder()
        {
            _headers = new Dictionary<string, string>();
            _requestParams = new List<RequestParam>();
        }

        public HttpRequestBuilder Get(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Get;
            return this;
        }

        public HttpRequestBuilder Post(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Post;
            return this;
        }

        public HttpRequestBuilder Put(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Put;
            return this;
        }

        public HttpRequestBuilder Delete(string uri)
        {
            _uri = uri;
            _method = HttpMethod.Delete;
            return this;
        }

        public HttpRequestBuilder AddQueryParam(string name, string value)
        {
            if (!_requestParams.Any(x => x.Type == Enums.RequestParamType.QueryString && x.Key.TrimAndLower() == name.TrimAndLower()))
                _requestParams.Add(new RequestParam(name, value, Enums.RequestParamType.QueryString));
            return this;
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

        public HttpRequestBuilder AddCancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
            return this;
        }

        public HttpRequestBuilder AddAuthorizationHeader(string name, string value)
        {
            _headers.Add("Authorization", value);
            return this;
        }

        public HttpRequestBuilder AddHeader(string name, string value)
        {
            _headers.Add(name, value);
            return this;
        }

        public HttpRequestBuilder AddBody(object body)
        {
            _body = body;
            return this;
        }

        public Task<Response> MakeRequestAsync()
        {
            Validate();
            
            return Helper.MakeHttpRequest(Utils.BuildUrl(_uri, _requestParams), 
                _method, 
                _body != null ? JsonConvert.SerializeObject(_body) : string.Empty, 
                _cancellationToken,
                Utils.BuildHeaderConfig(_headers));
        }

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
