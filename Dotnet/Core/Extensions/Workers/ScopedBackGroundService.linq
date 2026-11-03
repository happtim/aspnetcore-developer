<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var builder = Host.CreateApplicationBuilder();

builder.Services.AddHostedService<ConsumeScopedServiceHostedService>();
builder.Services.AddScoped<IScopedProcessingService,ScopedProcessingService>();

var app = builder.Build();

app.Run();


public class ConsumeScopedServiceHostedService : BackgroundService
{
	private readonly ILogger<ConsumeScopedServiceHostedService> _logger;

	public ConsumeScopedServiceHostedService(IServiceProvider services,
		ILogger<ConsumeScopedServiceHostedService> logger)
	{
		Services = services;
		_logger = logger;
	}

	public IServiceProvider Services { get; }

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation(
			"Consume Scoped Service Hosted Service running.");

		await DoWork(stoppingToken);
	}

	private async Task DoWork(CancellationToken stoppingToken)
	{
		_logger.LogInformation(
			"Consume Scoped Service Hosted Service is working.");

		using (var scope = Services.CreateScope())
		{
			var scopedProcessingService =
				scope.ServiceProvider
					.GetRequiredService<IScopedProcessingService>();

			await scopedProcessingService.DoWork(stoppingToken);
		}
	}

	public override async Task StopAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation(
			"Consume Scoped Service Hosted Service is stopping.");

		await base.StopAsync(stoppingToken);
	}
}

internal interface IScopedProcessingService
{
	Task DoWork(CancellationToken stoppingToken);
}

internal class ScopedProcessingService : IScopedProcessingService
{
	private int executionCount = 0;
	private readonly ILogger _logger;

	public ScopedProcessingService(ILogger<ScopedProcessingService> logger)
	{
		_logger = logger;
	}

	public async Task DoWork(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			executionCount++;

			_logger.LogInformation(
				"Scoped Processing Service is working. Count: {Count}", executionCount);

			await Task.Delay(2_000, stoppingToken);
		}
	}
}