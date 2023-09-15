<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Logging</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
</Query>

var builder = Host.CreateDefaultBuilder();

builder.ConfigureLogging((hostBuilderContext,loggingBuilder) => 
{
	//{
	//	"Logging": {
	//		"LogLevel": {
	//			"Default": "Information",
	//		      "Microsoft": "Warning",
	//		      "Microsoft.Hosting.Lifetime": "Information"
	//	
	//		}
	//	}
	//}

	//“Default”、“Microsoft”和“Microsoft.Hosting.Lifetime”日志级别类别是指定的。
	//如果未明确指定其他类别，则“Default”值适用于所有类别，实际上使所有类别的默认值为“信息”。您可以通过为类别指定值来覆盖此行为。
	//“Microsoft”类别适用于以“Microsoft”开头的所有类别。
	//“Microsoft”类别以警告及更高的日志级别记录。
	//“Microsoft.Hosting.Lifetime”类别比“Microsoft”类别更具体，所以“Microsoft.Hosting.Lifetime”类别以“信息”及更高的日志级别记录。


	//选择与Provider或其别名匹配的所有规则。如果找不到匹配项，请选择默认的规则。
	//从上一步的结果中选择具有最长匹配类别前缀的规则。如果找不到匹配项，请选择所有未指定类别的规则。
	//如果选择了多个规则，请选择最后一个。
	//如果未选择任何规则，请使用LoggingBuilderExtensions.SetMinimumLevel(ILoggingBuilder, LogLevel)来指定最低日志记录级别。
	loggingBuilder.AddFilter("System", LogLevel.Debug);
	loggingBuilder.AddFilter("Default",LogLevel.Information);
	loggingBuilder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information );
	loggingBuilder.AddFilter("Worker",LogLevel.Information);
	
	loggingBuilder.SetMinimumLevel(LogLevel.Warning);
	
});

builder.ConfigureServices((hostingContext, service) => 
{
	service.AddHostedService<Worker>();
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