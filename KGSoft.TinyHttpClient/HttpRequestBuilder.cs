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

        public HttpRequestBuilder AddFormParam(string name, string value)
        {
            if (!_requestParams.Any(x => x.Type == Enums.RequestParamType.FormEncoded && x.Key.TrimAndLower() == name.TrimAndLower()))
                _requestParams.Add(new RequestParam(name, value, Enums.RequestParamType.FormEncoded));
            return this;
        }

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

            return Helper.MakeHttpRequest(BuildUrl(), _method, _body != null ? JsonConvert.SerializeObject(_body) : string.Empty, _cancellationToken, BuildHeaderConfig());
        }

        public Task<Response<T>> MakeRequestAsync<T>()
        {
            Validate();

            return Helper.MakeHttpRequest<T>(BuildUrl(), _method, _body != null ? JsonConvert.SerializeObject(_body) : string.Empty, _cancellationToken, BuildHeaderConfig());
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_uri))
                throw new MissingUriException();
            if (_method == null)
                throw new MissingHttpMethodException();
        }

        private string BuildUrl()
        {
            if (!_requestParams.Any())
                return _uri;

            var sb = new StringBuilder();
            sb.Append(_uri.EnsureDoesNotEndWith("/"));

            foreach (var p in _requestParams)
                sb.AppendFormat("{0}{1}={2}",
                    p == _requestParams.First() ? "?" : "&",
                    p.Key,
                    p.Value);

            return sb.ToString();
        }

        private HeaderConfig BuildHeaderConfig()
        {
            if (!_headers.Any())
                return null;
            else
            {
                var cfg = new HeaderConfig
                {
                    CustomHeaders = new Dictionary<string, string>()
                };

                foreach (var h in _headers)
                    cfg.CustomHeaders.Add(h.Key, h.Value);

                return cfg;
            }
        }
    }
}
