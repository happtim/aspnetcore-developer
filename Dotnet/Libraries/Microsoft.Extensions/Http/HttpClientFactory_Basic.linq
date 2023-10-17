<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>Microsoft.Net.Http.Headers</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// Create a new instance of the service collection
var serviceCollection = new ServiceCollection();

//注册HttpClientFactory： 在Startup类的ConfigureServices方法中，添加HttpClientFactory的注册：
serviceCollection.AddHttpClient();

// Build the service provider and get an instance of the service
var serviceProvider = serviceCollection.BuildServiceProvider();
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();


//方法1 通过HttpClientFactory创建HttpClient： 
//在需要使用HttpClient的地方，通过依赖注入获取HttpClientFactory，并使用CreateClient方法创建HttpClient：

var httpClient = httpClientFactory.CreateClient();
// using Microsoft.Net.Http.Headers;
// The GitHub API requires two headers.
httpClient.DefaultRequestHeaders.Add(
	HeaderNames.Accept, "application/vnd.github.v3+json");
httpClient.DefaultRequestHeaders.Add(
	HeaderNames.UserAgent, "HttpRequestsSample");
await httpClient.GetStringAsync("https://api.github.com/").Dump("way1");