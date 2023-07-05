<Query Kind="Program">
  <NuGetReference Version="3.0.1">Serilog</NuGetReference>
  <NuGetReference Version="4.1.0">Serilog.Sinks.Console</NuGetReference>
  <NuGetReference Version="5.0.0">Serilog.Sinks.File</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>Serilog.Core</Namespace>
  <Namespace>Serilog.Events</Namespace>
  <Namespace>Serilog.Filters</Namespace>
</Query>

void Main()
{
	//Creating a logger 创建一个空的Logger 没有任何接受方
	Log.Logger = new LoggerConfiguration().CreateLogger();
	Log.Information("No one listens to me!");
	
	// Finally, once just before the application exits...
	Log.CloseAndFlush();
	
	
	//Sink  日志事件sink记录日志到不同的截止，如控制台，文件，数据库。
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console()
		.CreateLogger();
	
	Log.Information("Ah, there you are!");
	
	
	//Multiple sinks 一个logger允许多个sinks
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console()
		.WriteTo.File("log-linqpad-.txt", rollingInterval: RollingInterval.Day)
		.CreateLogger();
	
	Log.Information("Ah, Multiple sinks!");
	
	
	//Output templates 文本输出格式sink可以使用outputTemplate 参数控制output格式
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console()
		.WriteTo.File("log-linqpad-.txt",
			rollingInterval: RollingInterval.Day, 
			outputTemplate : "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
		.CreateLogger();
		
	Log.Information("Ah, outputTemplate!");
	
	//Minimum level 允许记录最小的日志级别
	// Verbose < Debug < Information < Warning < Error < Fatal
	// 默认使用 Information
	Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.Console()
		.CreateLogger();
		
	Log.Logger.Verbose("this is a Verbose log.");
	Log.Logger.Debug("this is a Verbose log.");
	
	//Overriding Minimum level per sink 有时候需要对每个sink重写输出级别 使用参数： restrictedToMinimumLevel
	Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.File("log-linqpad-.txt", rollingInterval: RollingInterval.Day)
		.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
		.CreateLogger();
	
	//Enrichers 日志加强 可以增加，修改log event中的属性。
	Log.Logger = new LoggerConfiguration()
		.Enrich.With(new ThreadIdEnricher())
		.Enrich.WithProperty("Version", "1.0.0") //如果这个属性是静态可以初始化设置。
		.WriteTo.Console(
			outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Version} {Message}{NewLine}{Exception}")
		.CreateLogger();
		
	Log.Logger.Information("Enrich add ThreadId");

	//Filters 使用可以使用Filter过滤
	Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.Filter.ByExcluding(Matching.WithProperty<int>("ThreadId", p => p != 20)) //可以筛选不是某个特定线程日志。
	.CreateLogger();

	//Sub-loggers 子logger 为了更好的控制logger输出，提供了要给pipeline用来输出log。
	Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.Logger(lc => lc
		.Filter.ByIncludingOnly(Matching.WithProperty<int>("ThreadId", p => p == 20)) //可以打印等于20线程id的日志达到文件。
		.WriteTo.File("log-linqpad-.txt"))
	.CreateLogger();
}

class ThreadIdEnricher : ILogEventEnricher
{
	public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
	{
		logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
				"ThreadId", Thread.CurrentThread.ManagedThreadId));
	}
}
