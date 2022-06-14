
# KGSoft.TinyHttpClient
A lightweight, highly compatible .NET library for simplifying the consumption of REST APIs (via both static client or Fluent API)

This library was created out if necessity, as it contains code we seemed to be duplicating throughout every Xamarin project we wrote. We have tried to encapsulate deserialization and the reading of HttpResponseMessages into this handy library. It has since grown into a handy library for server-side API consumption as well, hence the newly added v2.x.x Fluent API feature.

This library has two modes of use, each suited to different implementations. 
**Mode 1 (Static Client)** is better suited to client-side (ie. mobile/desktop apps) where you have a single static context for headers / token refreshing etc. 
**Mode 2 (Fluent API)** is better suited to server-side API consumption via the new Fluent API implementation.


## Features

* Encapsulation of types expected from an HttpRequest (Request and Request of T)
* Logging capability
* Global set and forget config (With the ability to manipulate headers on a request-by-request basis if needed)
* Re-use of HttpClient (HttpClient pool functionality coming soon)
* Pre-request action and async func (useful for ADAL requests where you need to get/refresh an ADAL token on every request)
* 401 Callback action and async func (useful for token expiry situations and triggering auth/refresh flow in client-side apps)
* **NEW: Fluent API for easy server-side use**

## Coming soon

* Form-encoded param support

## Nuget
https://www.nuget.org/packages/KGSoft.TinyHttpClient/

## Further Reading
https://www.mr-kg.com/kgsoft-tinyhttpclient-a-smarter-way-to-consume-your-apis/

## Usage

#### Simple GET request, expecting a response of T
###### Mode 1 (Static Client ie. Mobile/Desktop Apps)

`await Helper.GetAsync<SomeObject>("some http endpoint");`
###### Mode 2 (Fluent API)

    var response = await new HttpRequestBuilder()
				                .Get("some http endpoint")
				                .MakeRequestAsync<SomeObject>();

#### Simple POST request, not expecting an object to be returned

###### Mode 1 (Static Client ie. Mobile/Desktop Apps)
`await Helper.PostAsync("some http endpoint", "{ some json object }");`

###### Mode 2 (Fluent API)
    var response = await new HttpRequestBuilder()
                .Post("some http endpoint")
                .AddBody(new data() { some object })
                .MakeRequestAsync();

#### Simple POST request, expecting a response of T

###### Mode 1 (Static Client ie. Mobile/Desktop Apps)
`await Helper.PostAsync<SomeObject>("some http endpoint", "{ some json object }");`

###### Mode 2 (Fluent API)

    var response = await new HttpRequestBuilder()
                .Post("some http endpoint")
                .AddBody(new data() { some object })
                .MakeRequestAsync<SomeObject>();

#### Mode 1 Special Features
##### Pre-request Token Acquisition/Refresh Using Microsoft.IdentityModel.Clients.ActiveDirectory

```csharp
/// <summary>
/// Our method to get and set our ADAL AccessToken
/// </summary>
/// <returns></returns>
private async Task GetAuthBearerToken()
{
    var authority = "";
    var resource = "";
    var clientId = "";
    var redirectUri = "";
    var extraQueryParams = "";

    var authContext = new AuthenticationContext(authority);
    PlatformParameters platform = new PlatformParameters(PromptBehavior.Auto);

    // Call to the ADAL to get a token
    var result = await authContext.AcquireTokenAsync(
         resource,
         clientId,
         new Uri(redirectUri),
         platform,
         UserIdentifier.AnyUser,
         extraQueryParams);

    if (result != null) // We get a token, and we set it as the AuthHeader for our HttpClient in the global config
        HttpConfig.DefaultAuthHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
}


[TestMethod]
public async Task Test_PreAuth()
{
    // Set the PreRequestAuth Function here and forget about it
    HttpConfig.PreRequestAuthAsyncFunc = GetAuthBearerToken;
    var response = await Helper.GetAsync("some protected API");
}
```

#### For more examples, please consult the unit test project included in the repo
