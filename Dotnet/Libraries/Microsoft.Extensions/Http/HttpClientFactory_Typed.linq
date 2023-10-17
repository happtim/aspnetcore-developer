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

serviceCollection.AddHttpClient<IMyHttpClient, MyHttpClient>();

// Build the service provider and get an instance of the service
var serviceProvider = serviceCollection.BuildServiceProvider();
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

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
