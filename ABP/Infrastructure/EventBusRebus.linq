<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.AspNetCore</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.BackgroundWorkers</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.EventBus</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.EventBus.Rebus</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.AspNetCore</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.BackgroundWorkers</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.EventBus</Namespace>
  <Namespace>Volo.Abp.EventBus.Local</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Threading</Namespace>
  <Namespace>Volo.Abp.EventBus.Rebus</Namespace>
  <Namespace>Volo.Abp.EventBus.Distributed</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>Rebus.Persistence.InMem</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

//本地事件总线允许服务发布和订阅进程内事件。这意味着如果两个服务（发布者和订阅者）在同一个进程中运行，它就是适用的。

var builder = WebApplication.CreateBuilder();

builder.Host.UseAutofac();

await builder.AddApplicationAsync<AppModule>();

var app = builder.Build();

await app.InitializeApplicationAsync();

await app.RunAsync();


[DependsOn(typeof(AbpAspNetCoreModule))]
[DependsOn(typeof(AbpAutofacModule))] //Add dependency to ABP Autofac module
[DependsOn(typeof(AbpBackgroundWorkersModule))]
[DependsOn(typeof(AbpEventBusRebusModule))]
public class AppModule : AbpModule
{
	public override void PreConfigureServices(ServiceConfigurationContext context)
	{
		PreConfigure<AbpRebusEventBusOptions>(options =>
		{
			options.Configurer = rebusConfigurer =>
			{
				rebusConfigurer.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "eventbus"));
				rebusConfigurer.Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()));
			};
		});
	}

	public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
	{
		await context.AddBackgroundWorkerAsync<TestWorker>();
	}

	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var app = context.GetApplicationBuilder();
		var env = context.GetEnvironment();

		// Configure the HTTP request pipeline.
		if (env.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();
		app.UseConfiguredEndpoints();
	}
}

public class TestWorker : AsyncPeriodicBackgroundWorkerBase
{
	public TestWorker(
			AbpAsyncTimer timer,
			IServiceScopeFactory serviceScopeFactory
		) : base(
			timer,
			serviceScopeFactory)
	{
		Timer.Period = 1000; //1 seconds
	}

	protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
	{
		IDistributedEventBus  distributedEventBus = workerContext.ServiceProvider.GetRequiredService<IDistributedEventBus>();
		await distributedEventBus.PublishAsync(new GettingStarted { Value = DateTime.Now.ToString() },useOutbox:false);
	}
}

public class MyHandler
		: IDistributedEventHandler<GettingStarted>,
		  ITransientDependency
{
	private readonly ILogger<MyHandler> _logger;
	
	public MyHandler(ILogger<MyHandler> logger)
	{
		_logger = logger;
	}
	
	public  Task HandleEventAsync(GettingStarted eventData)
	{
		_logger.LogInformation("Received Text: {Text}", eventData.Value);
		return Task.CompletedTask;
	}
}


//测试消息时间的异步与扇出性。
//public class MyHandler2
//		: IDistributedEventHandler<GettingStarted>,
//		  ITransientDependency
//{
//	private readonly ILogger<MyHandler2> _logger;
//
//	public MyHandler2(ILogger<MyHandler2> logger)
//	{
//		_logger = logger;
//	}
//
//	public async Task HandleEventAsync(GettingStarted eventData)
//	{
//		_logger.LogInformation("Received Text 2 Begin: {Text}", eventData.Value);
//		await Task.Delay(5000);
//		_logger.LogInformation("Received Text 2 End: {Text}", eventData.Value);
//	}
//}

public class GettingStarted
{
	public string Value { get; init; }

	public override string ToString()
	{
		return Value;
	}
}