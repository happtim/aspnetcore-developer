<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>


// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DemoModule>();

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();

[DependsOn(typeof(Demo2Module))]
public class DemoModule : AbpModule
{

	public DemoModule()
	{
		//关闭约定注册
		SkipAutoServiceRegistration = true;
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddType<HelloWorldService>();
		ExposedServiceExplorer.GetExposedServices(typeof(HelloWorldService)).Dump("HelloWorldService Expose Service");
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var helloworld = context.ServiceProvider.GetRequiredService<HelloWorld2Service>();
		helloworld.SayHelloAsync();
		
		foreach (var hello in context.ServiceProvider.GetRequiredService< IEnumerable< HelloWorld2Service>>())
		{
			hello.Dump();
		}
	}
}

public class Demo2Module : AbpModule
{
	public Demo2Module()
	{
		//关闭约定注册
		SkipAutoServiceRegistration = true;
	}
	
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddType<HelloWorld2Service>();
		ExposedServiceExplorer.GetExposedServices(typeof(HelloWorld2Service)).Dump("HelloWorld2Service Expose Service");
	}
}

public class HelloWorld2Service : ITransientDependency 
{
	public virtual Task SayHelloAsync()
	{
		Console.WriteLine("Hello World 2!");
		return Task.CompletedTask;
	}
}

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(HelloWorld2Service))]
public class HelloWorldService : HelloWorld2Service
{

	public override Task SayHelloAsync()
	{
		Console.WriteLine("Hello World 1!");
		return Task.CompletedTask;
	}
}
