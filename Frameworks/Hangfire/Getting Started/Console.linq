<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference Version="0.5.1">Hangfire.InMemory</NuGetReference>
  <Namespace>Hangfire</Namespace>
</Query>

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseColouredConsoleLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseInMemoryStorage();

BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));

using (var server = new BackgroundJobServer())
{
	Console.ReadLine();
}