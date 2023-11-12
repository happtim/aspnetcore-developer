<Query Kind="Statements">
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Http.Client</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging.Abstractions</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Http.Client</Namespace>
</Query>

#load ".\Contracts"

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
	services.AddApplicationAsync<MyConsoleAppModule>();
	
}).UseAutofac().UseConsoleLifetime();

var host = builder.Build();
await host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>().InitializeAsync(host.Services);

await host.RunAsync();

[DependsOn(
	typeof(AbpAutofacModule),
	typeof(AbpHttpClientModule)
)]
public class MyConsoleAppModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.Configure<AbpRemoteServiceOptions>(options =>
	   {
		   options.RemoteServices.Default =
			   new RemoteServiceConfiguration("https://localhost:5001/");
	   });

		//Create dynamic client proxies
		context.Services.AddHttpClientProxies(
			typeof(MyConsoleAppModule).Assembly
		);
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		var servcie = context.ServiceProvider.GetRequiredService<IHelloWorldService>();
		var result =  await servcie.GetCallHelloAsync();
		result.Dump();
	}
}