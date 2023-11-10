<Query Kind="Statements">
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference>Serilog.Extensions.Logging</NuGetReference>
  <NuGetReference>Serilog.Sinks.Console</NuGetReference>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Serilog</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((hostBuilderContext, services) =>
{
	services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
	services.AddHostedService<Worker>();
});

var app = builder.Build();

await app.RunAsync();

public sealed class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;

	public Worker(ILogger<Worker> logger) =>
		_logger = logger;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
			await Task.Delay(1_000, stoppingToken);
		}
	}
}