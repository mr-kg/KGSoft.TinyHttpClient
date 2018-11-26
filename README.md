# KGSoft.TinyHttpClient
A lightweight, highly compatible .NET Standard library for simplifying the consumption of REST APIs

This library was created out if necessity, as it contains code we seemed to be duplicating throughout every Xamarin project we wrote. We have tried to encapsulate deserialization and the reading of HttpResponseMessages into this handy library.

## Features

* Encapsulation of types expected from an HttpRequest (Request and Request of T)
* Logging capability
* Global set and forget config
* Re-use of HttpClient (HttpClient pool functionality coming soon)
* Pre-request async authentication (useful for ADAL requests where you need to get/refresh an ADAL token on every request)
  
## Usage

#### Simple GET request, expecting a response of T


`await Helper.GetAsync<SomeObject>("some http endpoint");`

#### Simple POST request, not expecting an object to be returned


`await Helper.PostAsync("some http endpoint", "{ some json object }");`


#### Simple POST request, expecting a response of T


`await Helper.PostAsync<SomeObject>("some http endpoint", "{ some json object }");`

#### Pre-request Token Acquisition/Refresh Using Microsoft.IdentityModel.Clients.ActiveDirectory

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
