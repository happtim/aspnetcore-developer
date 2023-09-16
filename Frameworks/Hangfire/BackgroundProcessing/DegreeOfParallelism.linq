<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference>Hangfire.InMemory</NuGetReference>
  <Namespace>Hangfire</Namespace>
</Query>

//后台作业由在 Hangfire Server 子系统内运行的专用工作线程池处理。启动后台作业服务器时，它会初始化池并启动固定数量的工作器。

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseColouredConsoleLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseInMemoryStorage();

BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));

var options = new BackgroundJobServerOptions
{
	// This is the default value
	WorkerCount = Environment.ProcessorCount * 5
};

using (var server = new BackgroundJobServer(options))
{
	Console.ReadLine();
}