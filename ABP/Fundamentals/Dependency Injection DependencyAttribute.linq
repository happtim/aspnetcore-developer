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
		//手动使用框架内注册，该注册接口要晚于依赖的Demo2Module注册，
		//相同接口要覆盖，如何不覆盖[Dependency(TryRegister = true)]
		context.Services.AddType<HelloWorldService>();
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var helloworld = context.ServiceProvider.GetRequiredService<IHelloWorldService>();
		helloworld.SayHelloAsync();
	}
}

public class Demo2Module : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		//手动使用框架内注册
		context.Services.AddType<HelloWorld2Service>();
	}
}

[Dependency(TryRegister = true)]
[ExposeServices(typeof(IHelloWorldService), typeof(HelloWorldService))]
public class HelloWorldService: IHelloWorldService,ITransientDependency
{

	public Task SayHelloAsync()
	{
		Console.WriteLine("Hello World!");
		return Task.CompletedTask;
	}
}

[ExposeServices(typeof(IHelloWorldService), typeof(HelloWorld2Service))]
public class HelloWorld2Service : IHelloWorldService,ITransientDependency
{

	public Task SayHelloAsync()
	{
		Console.WriteLine("Hello World 2!");
		return Task.CompletedTask;
	}
}

public interface IHelloWorldService
{
	Task SayHelloAsync();
}
