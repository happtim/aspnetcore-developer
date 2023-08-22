<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DemoModule>();

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

public class DemoModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("DemoModule Configuring");
	}
}
