using KGSoft.TinyHttpClient.Logging;
using KGSoft.TinyHttpClient.Model;
using KGSoft.TinyHttpClient.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace KGSoft.TinyHttpClient.Test;

[TestClass]
public class HttpRequestBuilderTests : TestBase
{
    [TestMethod]
    public async Task Test_GET()
    {
        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/users/2")
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_GET_Result()
    {
        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/users/2")
            .MakeRequestAsync<TestUser>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_GET_MissingUri()
    {
        try
        {
            await new HttpRequestBuilder()
                .Get(string.Empty)
                .MakeRequestAsync();

            Assert.Fail($"{nameof(MissingUriException)} was not thrown.");
        }
        catch (MissingUriException)
        {
        }
    }

    [TestMethod]
    public async Task Test_GET_MismatchedType()
    {
        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/users/2")
            .MakeRequestAsync<MismatchedType>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_PUT()
    {
        var response = await new HttpRequestBuilder()
            .Put($"{ApiBase}api/users/2")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_PUT_Result()
    {
        var response = await new HttpRequestBuilder()
            .Put($"{ApiBase}api/users/2")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync<TestUser>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_POST()
    {
        var response = await new HttpRequestBuilder()
            .Post($"{ApiBase}api/users")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_POST_Result()
    {
        var response = await new HttpRequestBuilder()
            .Post($"{ApiBase}api/users")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync<TestUser>();

        AssertResponseResult(response);
    }

    [TestMethod]
    public async Task Test_DELETE()
    {
        var response = await new HttpRequestBuilder()
            .Delete($"{ApiBase}api/users")
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_GET_CtorUri()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Get()
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_PUT_CtorUri()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Put()
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_POST_CtorUri()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users")
            .Post()
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_PATCH()
    {
        var response = await new HttpRequestBuilder()
            .Patch($"{ApiBase}api/users/2")
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_PATCH_CtorUri()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Patch()
            .AddBody(new Data() { FirstName = "Chewbacca" })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_DELETE_CtorUri()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users")
            .Delete()
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_HEAD()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Head()
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_OPTIONS()
    {
        var response = await new HttpRequestBuilder()
            .Options($"{ApiBase}api/users/2")
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_WithUri()
    {
        var response = await new HttpRequestBuilder()
            .WithUri($"{ApiBase}api/users/2")
            .Get()
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_WithAliases()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users")
            .Post()
            .WithHeader("X-Correlation-ID", "123")
            .WithHeader("X-Correlation-ID", "456")
            .WithBearerToken("XYZ")
            .WithBody(new Data() { FirstName = "Chewbacca" })
            .WithJsonSerializerSettings(new JsonSerializerSettings())
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_AddQueryParams_Enumerable_AllowsDuplicates()
    {
        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Get()
            .AddQueryParams(new List<KeyValuePair<string, string>>
            {
                new("page", "1"),
                new("page", "2")
            })
            .MakeRequestAsync();

        AssertResponse(response);
    }

    [TestMethod]
    public async Task Test_WithContentFactory_WithRetry()
    {
        var contentFactoryCount = 0;
        var retryPolicy = new ResiliencePipelineBuilder<Response>()
            .AddRetry(new RetryStrategyOptions<Response>
            {
                ShouldHandle = new PredicateBuilder<Response>()
                    .HandleResult(response => !response.IsSuccess),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.Zero
            })
            .Build();

        var response = await new HttpRequestBuilder($"{ApiBase}api/unknown/23")
            .Post()
            .WithContent(() =>
            {
                contentFactoryCount++;
                return new StringContent("{}");
            })
            .WithRetry(retryPolicy)
            .MakeRequestAsync();

        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual(3, contentFactoryCount);
    }

    [TestMethod]
    public async Task Test_WithLogger_DefaultsToAllRequests()
    {
        var logger = new TestLogger();

        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Get()
            .WithLogger(logger)
            .MakeRequestAsync();

        AssertResponse(response);
        Assert.AreEqual(2, logger.Messages.Count);
    }

    [TestMethod]
    public async Task Test_WithLogger_OnlyFailedRequests_DoesNotLogSuccess()
    {
        var logger = new TestLogger();

        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Get()
            .WithLogger(logger)
            .WithLogScope(Enums.LogScope.OnlyFailedRequests)
            .MakeRequestAsync();

        AssertResponse(response);
        Assert.AreEqual(0, logger.Messages.Count);
    }

    [TestMethod]
    public async Task Test_WithLogger_OnlyFailedRequests_LogsFailure()
    {
        var logger = new TestLogger();

        var response = await new HttpRequestBuilder($"{ApiBase}api/unknown/23")
            .Get()
            .WithLogger(logger)
            .WithLogScope(Enums.LogScope.OnlyFailedRequests)
            .MakeRequestAsync();

        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual(1, logger.Messages.Count);
    }

    [TestMethod]
    public async Task Test_WithLogger_MicrosoftGenericLogger_DefaultsToAllRequests()
    {
        var logger = new MicrosoftTestLogger<HttpRequestBuilderTests>();

        var response = await new HttpRequestBuilder($"{ApiBase}api/users/2")
            .Get()
            .WithLogger(logger)
            .MakeRequestAsync();

        AssertResponse(response);
        Assert.AreEqual(2, logger.Messages.Count);
    }

    [TestMethod]
    public async Task Test_WithHttpClient()
    {
        var handler = new TestHttpMessageHandler();
        using var httpClient = new HttpClient(handler);

        var response = await new HttpRequestBuilder("https://example.com/api/users/2")
            .Get()
            .WithHttpClient(httpClient)
            .MakeRequestAsync();

        AssertResponse(response);
        Assert.AreEqual(1, handler.SendCount);
        Assert.AreEqual("https://example.com/api/users/2", handler.LastRequestUri.ToString());
    }

    [TestMethod]
    public async Task Test_GET_WithRetry()
    {
        var retryCount = 0;
        var retryPolicy = new ResiliencePipelineBuilder<Response>()
            .AddRetry(new RetryStrategyOptions<Response>
            {
                ShouldHandle = new PredicateBuilder<Response>()
                    .HandleResult(response => !response.IsSuccess),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.Zero,
                OnRetry = args =>
                {
                    retryCount++;
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/unknown/23")
            .WithRetry(retryPolicy)
            .MakeRequestAsync();

        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual(2, retryCount);
    }

    [TestMethod]
    public async Task Test_GET_Result_WithRetry()
    {
        var retryCount = 0;
        var retryPolicy = new ResiliencePipelineBuilder<Response>()
            .AddRetry(new RetryStrategyOptions<Response>
            {
                ShouldHandle = new PredicateBuilder<Response>()
                    .HandleResult(response => !response.IsSuccess),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.Zero,
                OnRetry = args =>
                {
                    retryCount++;
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        var response = await new HttpRequestBuilder()
            .Get($"{ApiBase}api/unknown/23")
            .WithRetry(retryPolicy)
            .MakeRequestAsync<TestUser>();

        Assert.IsFalse(response.IsSuccess);
        Assert.AreEqual(2, retryCount);
    }

    private sealed class TestLogger : ILogger
    {
        public List<string> Messages { get; } = new();
        public List<Exception> Exceptions { get; } = new();

        public void LogMessage(string message)
        {
            Messages.Add(message);
        }

        public void LogException(Exception ex)
        {
            Exceptions.Add(ex);
        }
    }

    private sealed class MicrosoftTestLogger<T> : Microsoft.Extensions.Logging.ILogger<T>
    {
        public List<string> Messages { get; } = new();
        public List<Exception> Exceptions { get; } = new();

        public IDisposable BeginScope<TState>(TState state)
            => null;

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            => true;

        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            Microsoft.Extensions.Logging.EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (exception != null)
                Exceptions.Add(exception);

            Messages.Add(formatter(state, exception));
        }
    }

    private sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        public int SendCount { get; private set; }
        public Uri LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SendCount++;
            LastRequestUri = request.RequestUri;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ok\":true}")
            });
        }
    }
}
