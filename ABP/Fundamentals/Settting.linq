<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Settings</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Settings</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
</Query>

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DemoModule>(options => 
{
	options.UseAutofac();
});

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

[DependsOn(
	typeof(AbpSettingsModule),
	typeof(AbpAutofacModule))]
public class DemoModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("DemoModule Configuring");
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		//获取SettingDefinition
		var manager =  context.ServiceProvider.GetRequiredService<ISettingDefinitionManager>();
		manager.Get("Smtp.Host").Dump();
		
		//获取Value
		var settingProvider = context.ServiceProvider.GetRequiredService<ISettingProvider>();
		
		//Get a value as string.
		await settingProvider.GetOrNullAsync("Smtp.UserName").Dump();
		
		//Get a bool value and fallback to the default value (false) if not set.
		await settingProvider.GetAsync<bool>("Smtp.EnableSsl").Dump();
		
		//Get a bool value and fallback to the provided default value (true) if not set.
		await settingProvider.GetAsync<bool>("Smtp.EnableSsl", defaultValue: true);
		
		//Get a bool value with the IsTrueAsync shortcut extension method
		await settingProvider.IsTrueAsync("Smtp.EnableSsl").Dump();

		//Get an int value or the default value (0) if not set
		await settingProvider.GetAsync<int>("Smtp.Port").Dump();

		//Get an int value or null if not provided
		(await settingProvider.GetOrNullAsync("Smtp.Port"))?.To<int>().Dump();

	}

}

public class EmailSettingProvider : SettingDefinitionProvider
{
	public override void Define(ISettingDefinitionContext context)
	{
		context.Add(
			new SettingDefinition("Smtp.Host", "127.0.0.1"),
			new SettingDefinition("Smtp.Port", "25"),
			new SettingDefinition("Smtp.UserName"),
			new SettingDefinition("Smtp.Password", isEncrypted: true),
			new SettingDefinition("Smtp.EnableSsl", "false")
		);
	}
}