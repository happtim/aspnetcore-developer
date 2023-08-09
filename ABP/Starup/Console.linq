<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference Version="3.0.1">Serilog</NuGetReference>
  <NuGetReference Version="6.1.0">Serilog.AspNetCore</NuGetReference>
  <NuGetReference Version="4.1.0">Serilog.Sinks.Console</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging.Abstractions</Namespace>
  <Namespace>Serilog</Namespace>
  <Namespace>Serilog.Events</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
</Query>


//Log.Logger = new LoggerConfiguration()
//		  .MinimumLevel.Debug()
//		  .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//		  .Enrich.FromLogContext()
//		  .WriteTo.Console()
//		  .CreateLogger();

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices(services =>
{
	services.AddHostedService<MyConsoleAppHostedService>();
	services.AddApplicationAsync<MyConsoleAppModule>(options =>
	{
		options.Services.ReplaceConfiguration(services.GetConfiguration());
		//options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
	});
}).UseAutofac().UseConsoleLifetime();


var host = builder.Build();
await host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>().InitializeAsync(host.Services);

await host.RunAsync();

[DependsOn(
	typeof(AbpAutofacModule)
)]
public class MyConsoleAppModule : AbpModule
{
	public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		var logger = context.ServiceProvider.GetRequiredService<ILogger<MyConsoleAppModule>>();
		var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
		logger.LogInformation($"MySettingName => {configuration["MySettingName"]}");

		var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
		logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

		return Task.CompletedTask;
	}
}

public class MyConsoleAppHostedService : IHostedService
{
	private readonly IAbpApplicationWithExternalServiceProvider _abpApplication;
	private readonly HelloWorldService _helloWorldService;

	public MyConsoleAppHostedService(HelloWorldService helloWorldService, IAbpApplicationWithExternalServiceProvider abpApplication)
	{
		_helloWorldService = helloWorldService;
		_abpApplication = abpApplication;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await _helloWorldService.SayHelloAsync();
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await _abpApplication.ShutdownAsync();
	}
}

public class HelloWorldService : ITransientDependency
{
	public ILogger<HelloWorldService> Logger { get; set; }

	public HelloWorldService()
	{
		Logger = NullLogger<HelloWorldService>.Instance;
	}

	public Task SayHelloAsync()
	{
		Logger.LogInformation("Hello World!");
		return Task.CompletedTask;
	}
}