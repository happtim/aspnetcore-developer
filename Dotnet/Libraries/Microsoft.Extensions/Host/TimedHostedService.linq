<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var builder = Host.CreateApplicationBuilder();

builder.Services.AddHostedService<TimedHostedService>();

var app = builder.Build();

app.Run();

public class TimedHostedService : IHostedService, IDisposable
{
	private int executionCount = 0;
	private readonly ILogger<TimedHostedService> _logger;
	private Timer? _timer = null;

	public TimedHostedService(ILogger<TimedHostedService> logger)
	{
		_logger = logger;
	}

	public Task StartAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Timed Hosted Service running.");

		_timer = new Timer(DoWork, null, TimeSpan.Zero,
			TimeSpan.FromSeconds(2));

		return Task.CompletedTask;
	}

	private void DoWork(object? state)
	{
		var count = Interlocked.Increment(ref executionCount);

		_logger.LogInformation(
			"Timed Hosted Service is working. Count: {Count}", count);
	}

	public Task StopAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Timed Hosted Service is stopping.");

		_timer?.Change(Timeout.Infinite, 0);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_timer?.Dispose();
	}
}