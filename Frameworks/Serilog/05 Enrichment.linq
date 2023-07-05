<Query Kind="Statements">
  <NuGetReference Version="3.0.1">Serilog</NuGetReference>
  <NuGetReference Version="3.1.0">Serilog.Enrichers.Thread</NuGetReference>
  <NuGetReference Version="4.1.0">Serilog.Sinks.Console</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>Serilog.Context</Namespace>
</Query>

//可以通过各种方式使用属性丰富日志事件。NuGet 提供了许多预生成的扩充器：
//Install-Package Serilog.Enrichers.Thread

var log = new LoggerConfiguration()
	.Enrich.WithThreadId()
	.WriteTo.Console(outputTemplate: "[{Level:w3}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff} {ThreadId} {Message:lj}{NewLine}{Exception}")
	.CreateLogger();
	
log.Information("日志内容包含了线程信息,UI线程id为1。");

//LogContext
//Serilog.Context.LogContext可用于在环境“执行上下文”中动态添加和删除属性;例如，在事务期间写入的所有消息都可能带有该事务的 ID，依此类推。

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff}  {A} {B} {Message:lj}{NewLine}{Exception}")
	.CreateLogger();

Log.Logger.Information("No contextual properties");

using (LogContext.PushProperty("A", 1))
{
	Log.Logger.Information("Carries property A = 1");
	//将属性推送到上下文将覆盖任何具有相同名称的现有属性，直到释放从中返回的对象
	using (LogContext.PushProperty("A", 2))
	using (LogContext.PushProperty("B", 1))
	{
		Log.Logger.Information("Carries A = 2 and B = 1");
	}

	Log.Logger.Information("Carries property A = 1, again");
}