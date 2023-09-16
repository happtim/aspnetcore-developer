<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference>Hangfire.InMemory</NuGetReference>
  <Namespace>Hangfire</Namespace>
</Query>

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseColouredConsoleLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseInMemoryStorage();
			   
BackgroundJob.Enqueue<HangfireJob>(job => job.Execute() );

var options = new BackgroundJobServerOptions
{
	//默认的定时循环时间15s。将其设置成1s
	SchedulePollingInterval = TimeSpan.FromSeconds(1)
};

using (var server = new BackgroundJobServer(options))
{
	Console.ReadLine();
}

//全局配置
//GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });

[AutomaticRetry(Attempts = 3)]
public class HangfireJob
{
	public void Execute()
	{ 
		Console.WriteLine("Hello, world!");
		throw new Exception("意外情况");
	}
}