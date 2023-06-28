<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>



// Create a new instance of the service collection
var serviceCollection = new ServiceCollection();

// Add services to the collection
serviceCollection.AddSingleton<IMyService, MyService>();
serviceCollection.AddTransient<IOtherService, OtherService>();

// Build the service provider and get an instance of the service
var serviceProvider = serviceCollection.BuildServiceProvider();
var myService = serviceProvider.GetService<IMyService>();

// Use the service
myService.DoSomething();


public interface IMyService
{
	void DoSomething();
}

public class MyService : IMyService
{
	private readonly IOtherService _otherService;

	public MyService(IOtherService otherService)
	{
		_otherService = otherService;
	}

	public void DoSomething()
	{
		Console.WriteLine("MyService is doing something...");
		_otherService.DoSomethingElse();
	}
}

public interface IOtherService
{
	void DoSomethingElse();
}

public class OtherService : IOtherService
{
	public void DoSomethingElse()
	{
		Console.WriteLine("OtherService is doing something else...");
	}
}