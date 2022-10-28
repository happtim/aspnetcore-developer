<Query Kind="Statements">
  <NuGetReference>Serilog</NuGetReference>
  <NuGetReference>Serilog.Sinks.Console</NuGetReference>
  <Namespace>Serilog</Namespace>
</Query>

var position = new { Latitude = 25, Longitude = 134 };
var elapsedMs = 34;

//通过LoggerConfiguration 创建logger。 如果有多个logger需要创建多个
using var log = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateLogger();
log.Information("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

//通过Log.Logger静态方法方法访问。
log.Information("Hello, Serilog!");

//可以将一个log 赋值给静态的全局变量。
//Log.Logger = log;
Log.Information("The global logger has been configured");