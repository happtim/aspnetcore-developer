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

public abstract class AbpApplicationBase : IAbpApplication
{
	public Type StartupModuleType => throw new NotImplementedException();

	public IServiceCollection Services => throw new NotImplementedException();

	public IServiceProvider ServiceProvider => throw new NotImplementedException();

	public IReadOnlyList<IAbpModuleDescriptor> Modules => throw new NotImplementedException();

	public async Task ConfigureServicesAsync()
	{
		var context = new ServiceConfigurationContext(Services);

		foreach (var module in Modules)
		{
			if (module.Instance is AbpModule abpModule)
			{
				//abpModule.ServiceConfigurationContext = context;
			}
		}

		//PreConfigureServices
		foreach (var module in Modules.Where(m => m.Instance is IPreConfigureServices))
		{
			await((IPreConfigureServices)module.Instance).PreConfigureServicesAsync(context);
		}

		var assemblies = new HashSet<Assembly>();

		//ConfigureServices
		foreach (var module in Modules)
		{
			if (module.Instance is AbpModule abpModule)
			{
				//if (!abpModule.SkipAutoServiceRegistration)
				{
					var assembly = module.Type.Assembly;
					if (!assemblies.Contains(assembly))
					{
						Services.AddAssembly(assembly);
						assemblies.Add(assembly);
					}
				}
			}

			await module.Instance.ConfigureServicesAsync(context);

		}

		//PostConfigureServices
		foreach (var module in Modules.Where(m => m.Instance is IPostConfigureServices))
		{
			await((IPostConfigureServices)module.Instance).PostConfigureServicesAsync(context);
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public void Shutdown()
	{
		throw new NotImplementedException();
	}

	public Task ShutdownAsync()
	{
		throw new NotImplementedException();
	}
}
