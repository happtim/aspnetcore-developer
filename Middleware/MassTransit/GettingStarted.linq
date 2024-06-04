<Query Kind="Statements">
  <NuGetReference>MassTransit</NuGetReference>
  <Namespace>MassTransit</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//使用简单的内存传输方式。实现一个发布订阅的模式。

var builder = WebApplication.CreateBuilder();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GettingStartedConsumer>();
	//x.AddConsumer<GettingStartedConsumer2>();

    x.UsingInMemory((context,cfg) =>
    {
		cfg.ConfigureEndpoints(context);
	});
});
					
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();


public class Worker : BackgroundService
{
	readonly IBus _bus;

	public Worker(IBus bus)
	{
		_bus = bus;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await _bus.Publish(new GettingStarted.Contracts.GettingStarted { Value = $"The time is {DateTimeOffset.Now}" }, stoppingToken);

			await Task.Delay(1000, stoppingToken);
		}
	}
}

namespace GettingStarted.Contracts 
{
	public record GettingStarted()
	{
		public string Value { get; init; }
	}
}



public class GettingStartedConsumer :
	IConsumer<GettingStarted.Contracts.GettingStarted>
{
	readonly ILogger<GettingStartedConsumer> _logger;

	public GettingStartedConsumer(ILogger<GettingStartedConsumer> logger)
	{
		_logger = logger;
	}

	public Task Consume(ConsumeContext<GettingStarted.Contracts.GettingStarted> context)
	{
		_logger.LogInformation("Received Text: {Text}", context.Message.Value);
		return Task.CompletedTask;
	}

}

//测试扇出模式是否线程隔离。
public class GettingStartedConsumer2 :
	IConsumer<GettingStarted.Contracts.GettingStarted>
{
	readonly ILogger<GettingStartedConsumer> _logger;

	public GettingStartedConsumer2(ILogger<GettingStartedConsumer> logger)
	{
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<GettingStarted.Contracts.GettingStarted> context)
	{
		_logger.LogInformation("Received Text 2 Begin: {Text}", context.Message.Value);
		await Task.Delay(5000);
		_logger.LogInformation("Received Text 2 End: {Text}", context.Message.Value);
	}

}

