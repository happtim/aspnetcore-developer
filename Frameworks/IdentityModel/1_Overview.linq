<Query Kind="Statements">
  <NuGetReference Version="6.1.0">IdentityModel</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http</NuGetReference>
  <Namespace>IdentityModel.Client</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
</Query>


string tokenUrl = "https://localhost:5001/connect/token";

//方法1 直接使用httpclient
var client = new HttpClient();

var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
	Address = tokenUrl,
	ClientId = "client",
	ClientSecret = "secret"
});

response.AccessToken.Dump("Directly HttpClient");


//使用IHttpClientFactory创建client
// Create a new instance of the service collection
var serviceCollection = new ServiceCollection();

serviceCollection.AddHttpClient();
serviceCollection.AddHttpClient("token_client",
	client => client.BaseAddress = new Uri(tokenUrl));

serviceCollection.Configure<TokenClientOptions>(options =>
		{
			options.Address = tokenUrl;
			options.ClientId = "client";
			options.ClientSecret = "secret";
		});

serviceCollection.AddTransient(sp => sp.GetRequiredService<IOptions<TokenClientOptions>>().Value);

serviceCollection.AddHttpClient<TokenClient>();

// Build the service provider and get an instance of the service
var serviceProvider = serviceCollection.BuildServiceProvider();
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

//方法2 直接使用httpClientFactory 创建httpclient
client = httpClientFactory.CreateClient();

response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
	Address = tokenUrl,
	ClientId = "client",
	ClientSecret = "secret"
});

response.AccessToken.Dump("HttpClientFactory Create HttpClient");

//方法3 使用命名httpclient
client = httpClientFactory.CreateClient("token_client");

response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
	ClientId = "client",
	ClientSecret = "secret"
});

response.AccessToken.Dump("Named HttpClient");

//方法4 Typed HttpClient
var tokenClient =  serviceProvider.GetService<TokenClient>();
response = await tokenClient.RequestClientCredentialsTokenAsync();

response.AccessToken.Dump("Typed HttpClient");