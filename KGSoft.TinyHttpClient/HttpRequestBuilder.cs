using KGSoft.TinyHttpClient.Model;
using KGSoft.TinyHttpClient.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace KGSoft.TinyHttpClient;

public class HttpRequestBuilder
{
    private string _uri;
    private readonly Dictionary<string, string> _headers = new();
    private readonly List<RequestParam> _requestParams = new();
    private HttpMethod _method;
    private object _body;
    private CancellationToken _cancellationToken;
    private Func<HttpContent> _contentFactory;
    private AuthenticationHeaderValue _authHeader;
    private JsonSerializerSettings _serializerSettings;
    private ILogger _logger;
    private Enums.LogScope? _logScope;
    private ResiliencePipeline<Response> _retryPolicy;

    private bool HasFormParams => _requestParams.Any(x => x.Type == Enums.RequestParamType.FormEncoded);
    private IEnumerable<RequestParam> FormParams => _requestParams.Where(x => x.Type == Enums.RequestParamType.FormEncoded);
    private IEnumerable<RequestParam> QueryParams => _requestParams.Where(x => x.Type == Enums.RequestParamType.QueryString);

    /// <summary>
    /// Default ctor for the Request Builder
    /// </summary>
    public HttpRequestBuilder()
    {
    }

    /// <summary>
    /// Default ctor for the Request Builder with a URI
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    public HttpRequestBuilder(string uri)
    {
        _uri = uri;
    }

    /// <summary>
    /// Sets the URI to be targeted
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    /// <summary>
    /// Signifies the intent for a GET request
    /// </summary>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Get()
    {
        _method = HttpMethod.Get;
        return this;
    }

    /// <summary>
    /// Signifies the intent for a GET request
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Get(string uri)
    {
        WithUri(uri);
        return Get();
    }

    /// <summary>
    /// Signifies the intent for a HEAD request
    /// </summary>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Head()
    {
        _method = HttpMethod.Head;
        return this;
    }

    /// <summary>
    /// Signifies the intent for a HEAD request
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Head(string uri)
    {
        WithUri(uri);
        return Head();
    }

    /// <summary>
    /// Signifies the intent for a POST request
    /// </summary>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Post()
    {
        _method = HttpMethod.Post;
        return this;
    }

    /// <summary>
    /// Signifies the intent for a POST request
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Post(string uri)
    {
        WithUri(uri);
        return Post();
    }

    /// <summary>
    /// Signifies the intent for a PUT request
    /// </summary>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Put()
    {
        _method = HttpMethod.Put;
        return this;
    }

    /// <summary>
    /// Signifies the intent for a PUT request
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Put(string uri)
    {
        WithUri(uri);
        return Put();
    }

    /// <summary>
    /// Signifies the intent for a PATCH request
    /// </summary>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Patch()
    {
        _method = HttpMethod.Patch;
        return this;
    }

    /// <summary>
    /// Signifies the intent for a PATCH request
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Patch(string uri)
    {
        WithUri(uri);
        return Patch();
    }

    /// <summary>
    /// Signifies the intent for an OPTIONS request
    /// </summary>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Options()
    {
        _method = HttpMethod.Options;
        return this;
    }

    /// <summary>
    /// Signifies the intent for an OPTIONS request
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Options(string uri)
    {
        WithUri(uri);
        return Options();
    }

    /// <summary>
    /// Signifies the intent for a DELETE request
    /// </summary>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Delete()
    {
        _method = HttpMethod.Delete;
        return this;
    }

    /// <summary>
    /// Signifies the intent for a DELETE request
    /// </summary>
    /// <param name="uri">The URI to be targeted</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder Delete(string uri)
    {
        WithUri(uri);
        return Delete();
    }

    /// <summary>
    /// Adds HttpContent to the request
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public HttpRequestBuilder AddContent(HttpContent content)
        => WithContent(content);

    /// <summary>
    /// Sets HttpContent for the request
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public HttpRequestBuilder WithContent(HttpContent content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        _contentFactory = () => content;
        return this;
    }

    /// <summary>
    /// Adds an HttpContent factory to the request. Prefer this overload when using retry policies.
    /// </summary>
    /// <param name="contentFactory"></param>
    /// <returns></returns>
    public HttpRequestBuilder AddContent(Func<HttpContent> contentFactory)
        => WithContent(contentFactory);

    /// <summary>
    /// Sets an HttpContent factory for the request. Prefer this overload when using retry policies.
    /// </summary>
    /// <param name="contentFactory"></param>
    /// <returns></returns>
    public HttpRequestBuilder WithContent(Func<HttpContent> contentFactory)
    {
        _contentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
        return this;
    }

    /// <summary>
    /// Adds a Query String parameter to the request
    /// </summary>
    /// <param name="name">Key for the parameter</param>
    /// <param name="value">Value for the parameter</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder AddQueryParam(string name, string value)
    {
        _requestParams.Add(new RequestParam(name, value, Enums.RequestParamType.QueryString));
        return this;
    }

    /// <summary>
    /// Adds a range of query string parameters to the request
    /// </summary>
    /// <param name="keyValuePairs">The dictionaly of parameters to be added</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder AddQueryParams(Dictionary<string, string> keyValuePairs)
        => AddQueryParams((IEnumerable<KeyValuePair<string, string>>)keyValuePairs);

    /// <summary>
    /// Adds a range of query string parameters to the request
    /// </summary>
    /// <param name="keyValuePairs">The collection of parameters to be added</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder AddQueryParams(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        if (keyValuePairs == null)
            throw new ArgumentNullException(nameof(keyValuePairs));

        foreach (var kvp in keyValuePairs)
            AddQueryParam(kvp.Key, kvp.Value);
        return this;
    }

    /// <summary>
    /// Adds a Form Encoded parameter to the request
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public HttpRequestBuilder AddFormParam(string name, string value)
    {
        _requestParams.Add(new RequestParam(name, value, Enums.RequestParamType.FormEncoded));
        return this;
    }


    /// <summary>
    /// Adds cancellation token to the request
    /// </summary>
    /// <param name="CancellationToken">The CancellationToken to be added</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder AddCancellationToken(CancellationToken token)
        => WithCancellationToken(token);

    /// <summary>
    /// Sets the cancellation token for the request
    /// </summary>
    /// <param name="token">The CancellationToken to be used</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithCancellationToken(CancellationToken token)
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
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var headerParts = value.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return headerParts.Length == 2
            ? WithAuthorization(new AuthenticationHeaderValue(headerParts[0], headerParts[1]))
            : WithHeader("Authorization", value);
    }

    /// <summary>
    /// Sets the Authorization header for the request
    /// </summary>
    /// <param name="value">The authorization header value.</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithAuthorization(AuthenticationHeaderValue value)
    {
        _authHeader = value ?? throw new ArgumentNullException(nameof(value));
        _headers.Remove("Authorization");
        return this;
    }

    /// <summary>
    /// Sets a Bearer token Authorization header for the request
    /// </summary>
    /// <param name="token">The bearer token.</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithBearerToken(string token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        return WithAuthorization(new AuthenticationHeaderValue("Bearer", token));
    }

    /// <summary>
    /// Adds a header to the request
    /// </summary>
    /// <param name="name">The key for the header</param>
    /// <param name="value">The value for the header</param>
    /// <returns></returns>
    public HttpRequestBuilder AddHeader(string name, string value)
        => WithHeader(name, value);

    /// <summary>
    /// Sets a header on the request
    /// </summary>
    /// <param name="name">The key for the header</param>
    /// <param name="value">The value for the header</param>
    /// <returns></returns>
    public HttpRequestBuilder WithHeader(string name, string value)
    {
        if (string.Equals(name, "Authorization", StringComparison.OrdinalIgnoreCase))
            return AddAuthorizationHeader(value);

        _headers[name] = value;
        return this;
    }

    /// <summary>
    /// Adds a body object to the request
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public HttpRequestBuilder AddBody(object body)
        => WithBody(body);

    /// <summary>
    /// Sets a body object for the request
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public HttpRequestBuilder WithBody(object body)
    {
        _body = body;
        return this;
    }

    /// <summary>
    /// Sets Newtonsoft.Json serializer settings for the request body.
    /// </summary>
    /// <param name="serializerSettings">The serializer settings to use.</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithJsonSerializerSettings(JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings ?? throw new ArgumentNullException(nameof(serializerSettings));
        return this;
    }

    /// <summary>
    /// Sets a logger for this request. Defaults to logging all request activity unless WithLogScope is also specified.
    /// </summary>
    /// <param name="logger">The logger to use for this request.</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithLogger(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        return this;
    }

    /// <summary>
    /// Sets the logging scope for this request.
    /// </summary>
    /// <param name="logScope">The log scope to use for this request.</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithLogScope(Enums.LogScope logScope)
    {
        _logScope = logScope;
        return this;
    }

    /// <summary>
    /// Adds a Polly resilience pipeline to execute this request with retry behavior.
    /// </summary>
    /// <param name="retryPolicy">The Polly resilience pipeline to execute the request with.</param>
    /// <returns>HttpRequestBuilder</returns>
    public HttpRequestBuilder WithRetry(ResiliencePipeline<Response> retryPolicy)
    {
        _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
        return this;
    }

    /// <summary>
    /// Makes the Http Request, WITHOUT a return type expected
    /// </summary>
    /// <returns>Response</returns>
    public Task<Response> MakeRequestAsync()
    {
        Validate();

        return ExecuteWithRetryAsync(() => Helper.MakeHttpRequest(Utils.BuildUrl(_uri, QueryParams), 
            _method, 
            SerializeBody(),
            BuildContent(),
            _cancellationToken,
            Utils.BuildHeaderConfig(_headers, _authHeader, _logger, BuildLogScope())));
    }

    /// <summary>
    /// Makes the Http Request, WITH a return type expected
    /// </summary>
    /// <returns>Response<typeparamref name="T"/></returns>
    public Task<Response<T>> MakeRequestAsync<T>()
    {
        Validate();

        return ExecuteWithRetryAsync(() => Helper.MakeHttpRequest<T>(Utils.BuildUrl(_uri, QueryParams), 
            _method, 
            SerializeBody(),
            BuildContent(),
            _cancellationToken, 
            Utils.BuildHeaderConfig(_headers, _authHeader, _logger, BuildLogScope())));
    }

    private void Validate()
    {
        if (string.IsNullOrEmpty(_uri))
            throw new MissingUriException();
        if (_method == null)
            throw new MissingHttpMethodException();
        if (_contentFactory != null && HasFormParams)
            throw new ConflictingContentException();
    }

    private string SerializeBody()
        => _body != null ? JsonConvert.SerializeObject(_body, _serializerSettings) : string.Empty;

    private Enums.LogScope? BuildLogScope()
        => _logScope ?? (_logger != null ? Enums.LogScope.AllRequests : null);

    private HttpContent BuildContent()
    {
        if (_contentFactory != null)
            return _contentFactory();
        else if (HasFormParams)
            return new FormUrlEncodedContent(FormParams.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));

        return null;
    }

    private Task<Response> ExecuteWithRetryAsync(Func<Task<Response>> request)
    {
        if (_retryPolicy == null)
            return request();

        return _retryPolicy.ExecuteAsync(async token => await request(), _cancellationToken).AsTask();
    }

    private async Task<Response<T>> ExecuteWithRetryAsync<T>(Func<Task<Response<T>>> request)
    {
        if (_retryPolicy == null)
            return await request();

        var response = await _retryPolicy.ExecuteAsync(async token => await request(), _cancellationToken);
        return (Response<T>)response;
    }
}
