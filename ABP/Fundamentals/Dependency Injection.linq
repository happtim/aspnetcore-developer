<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging.Abstractions</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DemoModule>();

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

public class DemoModule : AbpModule
{
	public DemoModule()
	{
		//自动注册的关闭
		SkipAutoServiceRegistration = true;
	}
	
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		//如果设置了不自动注册，则需要手动注册。
		context.Services.AddTransient<HelloWorldService>();
		context.Services.AddTransient<IHelloWorld23Service,HelloWorld2Service>();
		
		//自动注册寻找类型的接口，接口一定要匹配类的名称，否则注册不成接口。
		new MyCustomConventionalRegistrar().AddType(context.Services,typeof(HelloWorld2Service));
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var helloworld = context.ServiceProvider.GetRequiredService<HelloWorldService>();
		helloworld.SayHelloAsync();

		var helloworld2 = context.ServiceProvider.GetRequiredService<IHelloWorld23Service>();
		helloworld2.SayHelloAsync();
	}
}

public class HelloWorldService
{

	public Task SayHelloAsync()
	{
		Console.WriteLine("Hello World!");
		return Task.CompletedTask;
	}
}

public interface IHelloWorld23Service { 
	
	Task SayHelloAsync();
}

//自动注册 Interfaces，
public class HelloWorld2Service: IHelloWorld23Service,ITransientDependency
{

	public Task SayHelloAsync()
	{
		Console.WriteLine("Hello World 2!");
		return Task.CompletedTask;
	}
}

public class MyCustomConventionalRegistrar : ConventionalRegistrarBase
{
	public override void AddType(IServiceCollection services, Type type)
	{
		GetExposedServiceTypes(type).Dump();
	}
}