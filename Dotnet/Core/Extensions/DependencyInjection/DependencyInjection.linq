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

//如果没有注册会返回null
var myService = serviceProvider.GetService<IMyService>();

//如果没有注册会抛异常
try
{
	var myService2 = serviceProvider.GetRequiredService<IMyService2>();
	
}catch(Exception ex)
{
	ex.Message.Dump();
}


// Use the service
myService.DoSomething();

public interface IMyService
{
	void DoSomething();
}

public interface IMyService2 : IMyService
{
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