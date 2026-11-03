<Query Kind="Statements">
  <NuGetReference>CliWrap</NuGetReference>
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Http</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>CliWrap</Namespace>
  <Namespace>CliWrap.Buffered</Namespace>
</Query>


// Create a new instance of the service collection
var serviceCollection = new ServiceCollection();

serviceCollection.AddHttpClient();

// Build the service provider and get an instance of the service
var serviceProvider = serviceCollection.BuildServiceProvider();

IHttpClientFactory factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

(await(
	Cli.Wrap("netstat").WithArguments("-ano") |
	Cli.Wrap("findstr").WithArguments("TIME_WAIT"))
	.ExecuteBufferedAsync()
).StandardOutput.Dump();


for (int i = 0; i < 20; i++)
{
	var client = factory.CreateClient();
	var result = await client.GetAsync("http://localhost:5000");
	var hello = await result.Content.ReadAsStringAsync();
	Console.WriteLine("收到信息：" + hello);
	
}

(await(
	Cli.Wrap("netstat").WithArguments("-ano") |
	Cli.Wrap("findstr").WithArguments("TIME_WAIT"))
	.ExecuteBufferedAsync()
).StandardOutput.Dump();