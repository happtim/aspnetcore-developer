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


serviceCollection.AddHttpClient("myClient", client =>
{
	client.BaseAddress = new Uri("https://api.github.com/");

	// using Microsoft.Net.Http.Headers;
	// The GitHub API requires two headers.
	client.DefaultRequestHeaders.Add(
		HeaderNames.Accept, "application/vnd.github.v3+json");
	client.DefaultRequestHeaders.Add(
		HeaderNames.UserAgent, "HttpRequestsSample");
});

// Build the service provider and get an instance of the service
var serviceProvider = serviceCollection.BuildServiceProvider();
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();


//方法2 命名HttpClient的创建： 
//如果需要创建多个具有不同配置的HttpClient，在注册时可以使用不同的名称：
//- 提供与具名客户端相同的功能，无需使用字符串作为键。
//- 在使用客户端时提供智能感知和编译器帮助。
//- 提供一个单一位置来配置和与特定的HttpClient交互。例如，可以使用单一类型的客户端：
//	- 对于单个后端终端点。
//	- 封装处理终端点的所有逻辑。
//- 与依赖注入(DI)一起使用，可以在应用程序中需要时进行注入。
var httpClient = httpClientFactory.CreateClient("myClient");
await httpClient.GetStringAsync("").Dump("way2");