<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Logging</NuGetReference>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

#load ".\MyLoggerProvider"


//在.NET Core中，抽象类 LoggingProvider 是所有日志提供程序的基类，它定义了一个抽象方法 CreateLogger，
//用于创建日志记录器实例。LoggingProvider 类的主要作用是充当所有日志提供程序的统一入口点，用于创建和管理日志记录器。

//日志记录器是通过 CreateLogger 方法创建的，每个日志提供程序的实现都需要实现该方法。
//创建的日志记录器用于实际记录日志消息，通过调用 Log 方法将日志消息传递给具体的目标（比如控制台、文件、数据库等）。

// 创建一个新的.NET Core的DI容器
var serviceProvider = new ServiceCollection()
	.AddLogging(builder =>
	{
				// 注册自定义的LoggerProvider
		builder.AddProvider(new MyLoggerProvider());
				// 设置最低的日志记录级别
		builder.SetMinimumLevel(LogLevel.Debug);
	})
	.BuildServiceProvider();

// 从容器中获取ILogger<T>实例,  Logger<T>构造函数中，获取ILoggerFactory获取Logger
var logger = serviceProvider.GetRequiredService<ILogger<Person>>();

// 使用ILogger记录日志
logger.LogInformation("这是一个信息日志");
logger.LogWarning("这是一个警告日志");
logger.LogError("这是一个错误日志");

class Person{}