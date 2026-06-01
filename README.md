# KGSoft.TinyHttpClient

A lightweight .NET HTTP utility for consuming REST APIs with either a simple static helper API or a fluent request builder.

KGSoft.TinyHttpClient wraps common `HttpClient` tasks such as sending requests, applying headers, reading response content, deserializing JSON, handling typed responses, logging, and running authentication callbacks.

> Targets .NET 10.

## Installation

Install from NuGet:

```bash
dotnet add package KGSoft.TinyHttpClient
```

NuGet package:

https://www.nuget.org/packages/KGSoft.TinyHttpClient/

## Features

- Static helper API for quick requests
- Fluent request builder for readable request composition
- Typed responses via `Response<T>`
- Automatic JSON deserialization
- Raw response body access as both `string` and `byte[]`
- Query string support with URL encoding
- JSON request body support
- Form-encoded request support
- Custom `HttpContent` support
- Global and per-request header configuration
- Per-request Polly retry support
- Configurable per-request JSON serialization settings
- Reused `HttpClient` with configurable pooled connection lifetime, idle timeout, and request timeout
- Logging support
- Pre-request sync/async hooks for token acquisition or refresh
- 401 sync/async callbacks for authentication expiry flows

## Basic Usage

### Static Helper API

Use the `Helper` class when you want concise one-line HTTP calls.

```csharp
var response = await Helper.GetAsync("https://example.com/api/users/1");

if (response.IsSuccess)
{
    Console.WriteLine(response.Message);
}
```

### Typed GET Request

```csharp
var response = await Helper.GetAsync<User>("https://example.com/api/users/1");

if (response.IsSuccess)
{
    User user = response.Result;
}
```

### POST Request

```csharp
var body = JsonConvert.SerializeObject(new
{
    first_name = "Jane",
    last_name = "Doe"
});

var response = await Helper.PostAsync("https://example.com/api/users", body);
```

### Typed POST Request

```csharp
var body = JsonConvert.SerializeObject(new
{
    first_name = "Jane",
    last_name = "Doe"
});

var response = await Helper.PostAsync<User>("https://example.com/api/users", body);

if (response.IsSuccess)
{
    User user = response.Result;
}
```

## Fluent API

Use `HttpRequestBuilder` when you want requests to read naturally and compose options per request.

### GET

```csharp
var response = await new HttpRequestBuilder()
    .Get("https://example.com/api/users/1")
    .MakeRequestAsync<User>();
```

You can also provide the URI to the builder constructor and call the HTTP verb without a URI:

```csharp
var response = await new HttpRequestBuilder("https://example.com/api/users/1")
    .Get()
    .MakeRequestAsync<User>();
```

Or set the URI fluently:

```csharp
var response = await new HttpRequestBuilder()
    .WithUri("https://example.com/api/users/1")
    .Get()
    .MakeRequestAsync<User>();
```

### Query Parameters

```csharp
var response = await new HttpRequestBuilder()
    .Get("https://example.com/api/users")
    .AddQueryParam("page", "1")
    .AddQueryParam("search", "jane doe")
    .MakeRequestAsync<UserSearchResult>();
```

### POST JSON Body

```csharp
var response = await new HttpRequestBuilder()
    .Post("https://example.com/api/users")
    .WithBody(new
    {
        first_name = "Jane",
        last_name = "Doe"
    })
    .MakeRequestAsync<User>();
```

### PATCH JSON Body

```csharp
var response = await new HttpRequestBuilder("https://example.com/api/users/1")
    .Patch()
    .WithBody(new
    {
        first_name = "Jane"
    })
    .MakeRequestAsync<User>();
```

### HEAD and OPTIONS

```csharp
var headResponse = await new HttpRequestBuilder("https://example.com/api/users/1")
    .Head()
    .MakeRequestAsync();

var optionsResponse = await new HttpRequestBuilder()
    .Options("https://example.com/api/users")
    .MakeRequestAsync();
```

### Form-Encoded Request

```csharp
var response = await new HttpRequestBuilder()
    .Post("https://example.com/oauth/token")
    .AddFormParam("grant_type", "client_credentials")
    .AddFormParam("client_id", "my-client-id")
    .AddFormParam("client_secret", "my-client-secret")
    .MakeRequestAsync<TokenResponse>();
```

### Custom Headers

```csharp
var response = await new HttpRequestBuilder()
    .Get("https://example.com/api/users/1")
    .WithHeader("X-Correlation-ID", correlationId)
    .WithBearerToken(accessToken)
    .MakeRequestAsync<User>();
```

Calling `WithHeader` for the same header name replaces the previous value.

### Cancellation Token

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

var response = await new HttpRequestBuilder()
    .Get("https://example.com/api/users/1")
    .WithCancellationToken(cts.Token)
    .MakeRequestAsync<User>();
```

### Custom Content

```csharp
var response = await new HttpRequestBuilder("https://example.com/upload")
    .Post()
    .WithContent(() => new ByteArrayContent(fileBytes))
    .MakeRequestAsync();
```

Prefer the `Func<HttpContent>` overload when using retry policies, because it creates fresh content for each retry attempt.

### JSON Serialization Settings

```csharp
var response = await new HttpRequestBuilder("https://example.com/api/users")
    .Post()
    .WithBody(user)
    .WithJsonSerializerSettings(new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    })
    .MakeRequestAsync<User>();
```

### Retry With Polly

```csharp
var retryPolicy = new ResiliencePipelineBuilder<Response>()
    .AddRetry(new RetryStrategyOptions<Response>
    {
        ShouldHandle = new PredicateBuilder<Response>()
            .HandleResult(response => !response.IsSuccess),
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1)
    })
    .Build();

var response = await new HttpRequestBuilder()
    .Get("https://example.com/api/users/1")
    .WithRetry(retryPolicy)
    .MakeRequestAsync<User>();
```

Builders are intended to compose and send one request. Create a new `HttpRequestBuilder` for each request so headers, body, content, and retry policy state do not carry over unexpectedly.

### Per-Request Logging

```csharp
var response = await new HttpRequestBuilder("https://example.com/api/users/1")
    .Get()
    .WithLogger(logger)
    .MakeRequestAsync<User>();
```

`WithLogger` accepts either `KGSoft.TinyHttpClient.Logging.ILogger` or `Microsoft.Extensions.Logging.ILogger`, including `ILogger<T>`.

`WithLogger` logs all request activity by default. Use `WithLogScope` to limit that request to failed responses:

```csharp
var response = await new HttpRequestBuilder("https://example.com/api/users/1")
    .Get()
    .WithLogger(logger)
    .WithLogScope(Enums.LogScope.OnlyFailedRequests)
    .MakeRequestAsync<User>();
```

## Responses

Requests return either `Response` or `Response<T>`.

```csharp
public class Response
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public byte[] Content { get; set; }
}

public class Response<T> : Response
{
    public T Result { get; set; }
}
```

`Message` contains the response body as a string.

`Content` contains the response body as bytes.

`Result` contains the deserialized response body when using `Response<T>`.

## Global Configuration

`HttpConfig` can be used to configure defaults shared by requests.

```csharp
HttpConfig.MediaTypeHeader = Constants.ApplicationJson;
HttpConfig.RequestTimeoutSeconds = 100;
HttpConfig.HttpClientPoolLifetimeMinutes = 5;
HttpConfig.HttpClientPoolIdleMinutes = 2;
```

### Default Authorization Header

```csharp
HttpConfig.DefaultAuthHeader =
    new AuthenticationHeaderValue("Bearer", accessToken);
```

### Global Custom Headers

```csharp
HttpConfig.CustomHeaders["X-App-Version"] = "1.0.0";
```

### Per-Request Header Configuration

```csharp
var config = new HeaderConfig
{
    AuthHeader = new AuthenticationHeaderValue("Bearer", accessToken),
    CustomHeaders = new Dictionary<string, string>
    {
        { "X-Correlation-ID", correlationId }
    }
};

var response = await Helper.GetAsync<User>(
    "https://example.com/api/users/1",
    config: config);
```

## Authentication Hooks

You can run logic before each request. This is useful for acquiring or refreshing tokens.

```csharp
HttpConfig.PreRequestAuthAsyncFunc = async () =>
{
    var accessToken = await tokenProvider.GetAccessTokenAsync();

    HttpConfig.DefaultAuthHeader =
        new AuthenticationHeaderValue("Bearer", accessToken);
};
```

The hook runs before request headers are applied, so changes made inside the hook affect the current request.

## 401 Callbacks

You can register callbacks that run when a response returns `401 Unauthorized`.

```csharp
HttpConfig.UnauthorizedResultAction = () =>
{
    Console.WriteLine("Unauthorized response received.");
};
```

Or use an async callback:

```csharp
HttpConfig.UnauthorizedResultAsyncFunc = async () =>
{
    await authService.RefreshSignInAsync();
};
```

## Logging

Implement `ILogger` and assign it to `HttpConfig.Logger`.

```csharp
public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}

HttpConfig.Logger = new ConsoleLogger();
HttpConfig.LogScope = Enums.LogScope.AllRequests;
```

Supported log scopes:

```csharp
Enums.LogScope.OnlyFailedRequests
Enums.LogScope.AllRequests
```

## Notes

KGSoft.TinyHttpClient intentionally keeps the API small. It is useful when you want a lightweight wrapper around `HttpClient` without adopting a larger REST client framework.

For more examples, see the test project in this repository.
