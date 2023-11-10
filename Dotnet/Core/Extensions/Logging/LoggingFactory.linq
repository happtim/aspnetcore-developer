<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Logging</NuGetReference>
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

#load ".\MyLoggerProvider"


ServiceCollection services = new ServiceCollection();
services.AddLogging(builder =>
	{
		builder.AddConsole();
		// 注册自定义的LoggerProvider
		builder.AddProvider(new MyLoggerProvider());
		// 设置最低的日志记录级别
		builder.SetMinimumLevel(LogLevel.Debug);
	});
	
ServiceProvider serviceProvider = services.BuildServiceProvider();
ILoggerFactory loggingFactory = serviceProvider.GetRequiredService<ILoggerFactory>();


// 使用loggerFactory创建一个ILogger实例
var logger = loggingFactory.CreateLogger<Person>();

// 使用ILogger记录日志
logger.LogInformation("这是一个信息日志");
logger.LogWarning("这是一个警告日志");
logger.LogError("这是一个错误日志");


class Person{}
