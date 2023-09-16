<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference>Hangfire.InMemory</NuGetReference>
  <NuGetReference Version="3.0.1">Serilog</NuGetReference>
  <NuGetReference>Serilog.Sinks.Console</NuGetReference>
  <Namespace>Hangfire</Namespace>
  <Namespace>Serilog</Namespace>
</Query>


Log.Logger = new LoggerConfiguration()
		.WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff}  {Message:lj}{NewLine}{Exception}")
		.CreateLogger();

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseSerilogLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseInMemoryStorage();

BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));

using (var server = new BackgroundJobServer())
{
	Console.ReadLine();
}