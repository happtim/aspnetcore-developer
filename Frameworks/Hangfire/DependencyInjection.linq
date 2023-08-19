<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire</NuGetReference>
  <NuGetReference>Hangfire.Autofac</NuGetReference>
  <NuGetReference>Hangfire.InMemory</NuGetReference>
  <Namespace>Hangfire</Namespace>
  <Namespace>Autofac</Namespace>
</Query>

var builder = new ContainerBuilder();

builder.RegisterType<EmailSender>().As<IEmailSender>();

GlobalConfiguration.Configuration
			   .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
			   .UseColouredConsoleLogProvider()
			   .UseSimpleAssemblyNameTypeSerializer()
			   .UseRecommendedSerializerSettings()
			   .UseAutofacActivator( builder.Build())
			   .UseInMemoryStorage();

BackgroundJob.Enqueue<IEmailSender>(x => x.Send(12,"hello world"));

using (var server = new BackgroundJobServer())
{
	Console.ReadLine();
}

public interface IEmailSender
{
	void Send(int userId, string message);
}

public class EmailSender : IEmailSender
{
	public void Send(int userId, string message)
	{
		Console.WriteLine($"send email to {userId} : {message}");
	}
}