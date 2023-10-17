<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

//ServiceScope 代表了一次作用域，它会在作用域结束时释放所有由该作用域创建的服务。
//ServiceScopeFactory 可以用来创建这个作用域并获取其中的服务。

var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<IMyService, MyService>();
var serviceProvider = serviceCollection.BuildServiceProvider();

// Create a scope factory
var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
//等同下面的方法
//serviceProvider.CreateScope()

// Create a new scope
using (var scope = scopeFactory.CreateScope())
{
	// Get an instance of my service
	var myService = scope.ServiceProvider.GetService<IMyService>();
	myService.DoSomething();
}


public interface IMyService
{
	void DoSomething();
}

public class MyService : IMyService,IDisposable
{
	public void Dispose()
	{
		Console.WriteLine("MyService is Disposable...");
	}

	public void DoSomething()
	{
		Console.WriteLine("MyService is doing something...");
	}
}