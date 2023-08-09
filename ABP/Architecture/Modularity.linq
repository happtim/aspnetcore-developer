<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
</Query>

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DomeModule>();

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

[DependsOn(typeof(Dome1Module))]
[DependsOn(typeof(Dome2Module))]
public class DomeModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("Dome Configuring");
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		Console.WriteLine("Dome Initializing");
	}

	public override void OnApplicationShutdown(ApplicationShutdownContext context)
	{
		Console.WriteLine("Dome Shutdown");
	}
}

[DependsOn(typeof(Dome3Module))]
public class Dome1Module : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("Dome1 Configuring");
	}
}

[DependsOn(typeof(Dome3Module))]
public class Dome2Module : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("Dome2 Configuring");
	}
}

public class Dome3Module : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("Dome3 Configuring");
	}
}