<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>


var serviceProvider = new ServiceCollection()
			   //暂时生存期服务是每次从服务容器进行请求时创建的。 这种生存期适合轻量级、 无状态的服务。
			   //.AddTransient<IMyService, MyService>() 
			   
			   //对于 Web 应用，指定了作用域的生存期指明了每个客户端请求（连接）创建一次服务。 
			   .AddScoped<IMyService, MyService>()
			   
			   //每一个后续请求都使用同一个实例。
			   //.AddSingleton<IMyService, MyService>()
			   .BuildServiceProvider();
			   

//scope 对用的 serviceProvider 只提供scope范围内
using (var scope = serviceProvider.CreateScope())
{
	var myService1 = scope.ServiceProvider.GetService<IMyService>();
	var myService2 = scope.ServiceProvider.GetService<IMyService>();
	Console.WriteLine($"myService1 guid: {myService1.GetMessage()}" );
	Console.WriteLine($"myService2 guid: {myService2.GetMessage()}" );

	using (var scope1 = serviceProvider.CreateScope())
	{
		var myService3 = scope1.ServiceProvider.GetService<IMyService>();
		var myService4 = scope1.ServiceProvider.GetService<IMyService>();
		Console.WriteLine($"myService3 guid: {myService3.GetMessage()}");
		Console.WriteLine($"myService4 guid: {myService4.GetMessage()}");
	}
}


public interface IMyService
{
	string GetMessage();
}

public class MyService : IMyService
{
	private readonly string _message;

	public MyService()
	{
		_message = Guid.NewGuid().ToString();
	}

	public string GetMessage()
	{
		return _message;
	}
}