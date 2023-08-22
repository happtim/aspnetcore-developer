<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Core</NuGetReference>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp.Reflection</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.DynamicProxy</Namespace>
</Query>

// 1: Create the ABP application container
using var application = await AbpApplicationFactory.CreateAsync<DemoModule>(options => 
{
	//OnRegistred
	//Property injection.
	//Dynamic proxying 
	options.UseAutofac();
});

// 2: Initialize/start the ABP Framework (and all the modules)
await application.InitializeAsync();

Console.WriteLine("ABP Framework has been started...");

// 3: Stop the ABP Framework (and all the modules)
await application.ShutdownAsync();


[DependsOn(typeof(AbpAutofacModule))]
public class DemoModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.OnRegistred(context =>
		{
			if (typeof(IHelloWorldService).IsAssignableFrom(context.ImplementationType))
			{
				context.Interceptors.TryAdd<MyLogInterceptor>();
			}
		});
	}

	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		Console.WriteLine("DemoModule Configuring");
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		var helloWorldService =  context.ServiceProvider.GetRequiredService<IHelloWorldService>();
		await helloWorldService.SayHelloAsync();
	}
}

[ExposeServices(typeof(IHelloWorldService), typeof(HelloWorld2Service))]
public class HelloWorld2Service : IHelloWorldService, ITransientDependency
{
	public ITypeFinder TypeFinder { get; set; }
	public virtual Task  SayHelloAsync()
	{
		Console.WriteLine("TypeFinder Count:" + TypeFinder.Types.Count());
		return Task.CompletedTask;
	}
}

public interface IHelloWorldService
{
	Task SayHelloAsync();
}

public class MyLogInterceptor : IAbpInterceptor, ITransientDependency
{
	public async Task InterceptAsync(IAbpMethodInvocation invocation)
	{
		Console.WriteLine("拦截成功前");
		await invocation.ProceedAsync();
		Console.WriteLine("拦截成功后");
	}
}
