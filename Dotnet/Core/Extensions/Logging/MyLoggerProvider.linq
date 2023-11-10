<Query Kind="Statements">
  <NuGetReference>Microsoft.Extensions.Logging</NuGetReference>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
</Query>

public class MyLoggerProvider : ILoggerProvider
{
	public ILogger CreateLogger(string categoryName)
	{
		return new MyLogger();
	}

	public void Dispose()
	{
		// 对于自定义的LoggerProvider，如果需要释放资源，可以在这里实现
	}
}

public class MyLogger : ILogger
{
	public IDisposable BeginScope<TState>(TState state)
	{
		// 这里可以返回一个实现IDisposable接口的对象，以支持范围作用域的日志记录
		return null;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		// 设置日志记录级别，返回一个bool值表示该级别的日志是否将被记录
		return true;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		// 实现日志记录的逻辑
		string message = formatter(state, exception);

		// 这里可以根据需要将日志输出到控制台、文件、数据库等
		Console.WriteLine($"[{logLevel}] {message}");
	}
}