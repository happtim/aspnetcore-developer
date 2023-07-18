<Query Kind="Statements">
  <NuGetReference Version="4.3.0">IdentityModel.AspNetCore</NuGetReference>
  <NuGetReference Version="6.1.0">Serilog.AspNetCore</NuGetReference>
  <Namespace>IdentityModel.Client</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Serilog</Namespace>
  <Namespace>Serilog.Sinks.SystemConsole.Themes</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>IdentityModel.AspNetCore.AccessTokenManagement</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


var builder = WebApplication.CreateBuilder();
Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.Console(theme: AnsiConsoleTheme.Code)
		.CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddAccessTokenManagement(options =>
{
	options.Client.Clients.Add(AccessTokenManagementDefaults.DefaultTokenClientName, new ClientCredentialsTokenRequest
	{
		Address = "https://localhost:5001/connect/token",
		ClientId = "client",
		ClientSecret = "secret",
		Scope = "api1" // optional
	});
	
	// 在IdentityModel库中，CacheLifetimeBuffer是一项选项，用于缓冲访问令牌的寿命。
	//它的作用是确保在访问令牌过期之前进行刷新，以避免由于网络延迟等原因导致访问令牌过期而导致的授权失败。
	options.Client.CacheLifetimeBuffer = 0;
});

builder.Services.AddClientAccessTokenHttpClient("client",configureClient: client =>
{
	client.BaseAddress = new Uri("https://localhost:6001");
});
	
builder.Services.AddHostedService<Worker>();
var app = builder.Build();
app.Urls.Add("https://locahost:7000");
app.Run();

public class Worker : BackgroundService
{
	private readonly IHttpClientFactory _clientFactory;
	private readonly ILogger<Worker> _logger;
	private readonly IClientAccessTokenManagementService _clientAccessTokenManagementService;
	private int i = 0;
	
	public Worker(
		ILogger<Worker> logger, 
		IHttpClientFactory factory,
		IClientAccessTokenManagementService clientAccessTokenManagementService
		)
	{
		_logger = logger;
		_clientFactory = factory;
		_clientAccessTokenManagementService = clientAccessTokenManagementService;
	}
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.Delay(2000, stoppingToken);
		
		while (!stoppingToken.IsCancellationRequested)
		{
			i++;
			Console.WriteLine("\n\n");
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
			HttpResponseMessage response = null;
			if(i%2 ==0)
			{
				//方法1
				var client = _clientFactory.CreateClient("client");
				response = await client.GetAsync("identity", stoppingToken);
			}
			else
			{
				//方法2
				var accessToken = await _clientAccessTokenManagementService.GetClientAccessTokenAsync(AccessTokenManagementDefaults.DefaultTokenClientName);
				var client = _clientFactory.CreateClient();
				
				// 创建一个HttpRequestMessage实例
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:6001/identity");
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				response = await client.SendAsync(request);
			}

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync(stoppingToken);
				_logger.LogInformation("API response: {response}", content);
			}
			else
			{
				_logger.LogError("API returned: {statusCode}", response.StatusCode);
			}
			
			await Task.Delay(5000, stoppingToken);
		}
	}
}