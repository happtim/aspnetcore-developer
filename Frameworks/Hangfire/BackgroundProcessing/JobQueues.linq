<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference>Hangfire.InMemory</NuGetReference>
  <Namespace>Hangfire</Namespace>
</Query>

//Hangfire可以处理多个队列。如果你想确定作业的优先级，
//或者在你的服务器上拆分处理（一些进程用于存档队列，另一些进程用于图像队列等）

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseColouredConsoleLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseInMemoryStorage();

for (int i = 0; i < 10; i++)
{
	BackgroundJob.Enqueue<HangfireJob>(job => job.Execute3());
	BackgroundJob.Enqueue<HangfireJob>(job => job.Execute2());
	BackgroundJob.Enqueue<HangfireJob>(job => job.Execute1());
}

//队列的运行顺序取决于具体的存储实现。例如，当我们使用Hangfire.SqlServer时，顺序由字母数字顺序定义，数组索引被忽略。
//使用 Hangfire.Pro.Redis 包时，数组索引很重要，索引较低的队列将首先被处理。
var options = new BackgroundJobServerOptions
{
	Queues = new[] { "alpha", "beta", "default" },
	WorkerCount = 1
	
};
using (var server = new BackgroundJobServer(options))
{
	Console.ReadLine();
}

public class HangfireJob
{
	[Queue("alpha")]
	public void Execute1()
	{
		Console.WriteLine("Hello, world! alpha");
	}
	
	[Queue("beta")]
	public void Execute2()
	{
		Console.WriteLine("Hello, world! beta");
	}
	[Queue("default")]
	public void Execute3()
	{
		Console.WriteLine("Hello, world! default");
	}
}
