<Query Kind="Statements">
  <NuGetReference Version="6.1.0">Serilog.AspNetCore</NuGetReference>
  <Namespace>Serilog</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Serilog.Extensions.Logging</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Console()
	.CreateLogger();
	
try
{
	Log.Information("Starting web application");

	var builder = WebApplication.CreateBuilder();

	builder.Host.UseSerilog(); // <-- Add this line
	
	//or
	
	//builder.Services.AddSingleton<ILoggerFactory>(_ => new SerilogLoggerFactory());

	var app = builder.Build();

	app.MapGet("/", () => "Hello World!");

	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}