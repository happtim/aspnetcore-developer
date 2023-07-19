<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>Microsoft.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// Create a new instance of the service collection
var serviceCollection = new ServiceCollection();

//注册HttpClientFactory： 在Startup类的ConfigureServices方法中，添加HttpClientFactory的注册：
//serviceCollection.AddHttpClient();

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


serviceCollection.AddHttpClient<IMyHttpClient, MyHttpClient>();

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

//方法2 命名HttpClient的创建： 
//如果需要创建多个具有不同配置的HttpClient，在注册时可以使用不同的名称：
//- 提供与具名客户端相同的功能，无需使用字符串作为键。
//- 在使用客户端时提供智能感知和编译器帮助。
//- 提供一个单一位置来配置和与特定的HttpClient交互。例如，可以使用单一类型的客户端：
//	- 对于单个后端终端点。
//	- 封装处理终端点的所有逻辑。
//- 与依赖注入(DI)一起使用，可以在应用程序中需要时进行注入。
httpClient =  httpClientFactory.CreateClient("myClient");
await httpClient.GetStringAsync("").Dump("way2");


//方法3 使用Typed HttpClient： 
//可以通过定义一个带有特定接口的HttpClient来进一步简化使用：
var myhttpClient = serviceProvider.GetService<IMyHttpClient>();
await myhttpClient.GetAspNetCoreDocsBranchesAsync().Dump("way3");


public interface IMyHttpClient
{
	Task<IEnumerable<GitHubBranch>?> GetAspNetCoreDocsBranchesAsync();
}

public class MyHttpClient : IMyHttpClient
{
	private readonly HttpClient _httpClient;

	public MyHttpClient(HttpClient httpClient)
	{
		_httpClient = httpClient;

		_httpClient.BaseAddress = new Uri("https://api.github.com/");

		// using Microsoft.Net.Http.Headers;
		// The GitHub API requires two headers.
		_httpClient.DefaultRequestHeaders.Add(
			HeaderNames.Accept, "application/vnd.github.v3+json");
		_httpClient.DefaultRequestHeaders.Add(
			HeaderNames.UserAgent, "HttpRequestsSample");
	}

	public async Task<IEnumerable<GitHubBranch>?> GetAspNetCoreDocsBranchesAsync() =>
	await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>(
		"repos/dotnet/AspNetCore.Docs/branches");
}

/// <summary>
/// A partial representation of a branch object from the GitHub API
/// </summary>
public class GitHubBranch
{
	[JsonPropertyName("name")]
	public string Name { get; set; }
}
