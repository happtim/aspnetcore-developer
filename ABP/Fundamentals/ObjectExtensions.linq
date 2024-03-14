<Query Kind="Statements">
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
	services.AddApplicationAsync<MyConsoleAppModule>(options =>
	{
		options.Services.ReplaceConfiguration(services.GetConfiguration());
		//options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
	});
	
}).UseAutofac().UseConsoleLifetime();


var host = builder.Build();

await host.RunAsync();

[DependsOn(
	typeof(AbpAutofacModule)
)]
public class MyConsoleAppModule : AbpModule
{
	public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		return Task.CompletedTask;
	}
}
