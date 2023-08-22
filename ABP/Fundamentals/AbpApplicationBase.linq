<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
</Query>

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
			await ((IPreConfigureServices)module.Instance).PreConfigureServicesAsync(context);
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
			await ((IPostConfigureServices)module.Instance).PostConfigureServicesAsync(context);
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