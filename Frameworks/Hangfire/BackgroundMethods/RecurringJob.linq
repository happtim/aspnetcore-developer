<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference>Hangfire.InMemory</NuGetReference>
  <Namespace>Hangfire</Namespace>
  <Namespace>Hangfire.Common</Namespace>
</Query>

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseColouredConsoleLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseInMemoryStorage();

RecurringJob.AddOrUpdate("easyjob", () => Console.Write("Easy!\n"), Cron.Minutely);
RecurringJob.AddOrUpdate("powerfuljob", () => Console.Write("Easy!!\n"), "*/1 * * * * *");


RecurringJob.RemoveIfExists("powerfuljob");
RecurringJob.TriggerJob("easyjob");

var manager = new RecurringJobManager();
manager.AddOrUpdate("jobManager", Job.FromExpression(() => Console.Write("Easy!!!\n")), "*/1 * * * * *");


var options = new BackgroundJobServerOptions
{
	//默认的定时循环时间15s。将其设置成1s
	SchedulePollingInterval = TimeSpan.FromSeconds(1)
};
using (var server = new BackgroundJobServer(options))
{
	Console.ReadLine();
}