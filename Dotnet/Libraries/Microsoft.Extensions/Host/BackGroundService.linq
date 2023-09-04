<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var builder = Host.CreateApplicationBuilder();

builder.Services.AddHostedService<MyBackgroundService>();

var app = builder.Build();

app.Run();

// 创建一个后台任务的类
public class MyBackgroundService : BackgroundService
{
	private readonly ILogger<MyBackgroundService> _logger;

	public MyBackgroundService(ILogger<MyBackgroundService> logger)
	{
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			// 执行后台任务

			_logger.LogInformation("Background task is running.");

			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // 每隔1秒执行一次任务
		}
	}
}