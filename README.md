# KGSoft.TinyHttpClient
A lightweight, highly compatible .NET Standard library for simplifying the consumption of REST APIs

This library was created out if necessity, as it contains code we seemed to be duplicating throughout every Xamarin project we wrote. We have tried to encapsulate deserialization and the reading of HttpResponseMessages into this handy library.

## Features

* Encapsulation of types expected from an HttpRequest (Request and Request of T)
* Logging capability
* Global set and forget config
* Re-use of HttpClient (HttpClient pool functionality coming soon)
  
## Usage

#### Simple GET request, expecting a response of T


`await Helper.GetAsync<SomeObject>("some http endpoint");`

#### Simple POST request, not expecting an object to be returned


`await Helper.PostAsync("some http endpoint", "{ some json object }");`


#### Simple POST request, expecting a response of T


`await Helper.PostAsync<SomeObject>("some http endpoint", "{ some json object }");`


#### For more examples, please consult the unit test project included in the repo
