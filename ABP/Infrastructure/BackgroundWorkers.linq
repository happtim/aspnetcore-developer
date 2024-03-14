<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.AspNetCore</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.BackgroundWorkers</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.AspNetCore</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.BackgroundWorkers</Namespace>
  <Namespace>Volo.Abp.Threading</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// BackgroundWorker 使用AbpAsyncTimer （线程安全的），当一个次定时完成才启动下一个。
// 在DoWorkAsync 方法中使用 PeriodicBackgroundWorkerContext 去使用依赖注入，这样当定时器完成时，才能释放Scope。

var builder = WebApplication.CreateBuilder();

builder.Host.UseAutofac();

await builder.AddApplicationAsync<AppModule>();

var app = builder.Build();

await app.InitializeApplicationAsync();

await app.RunAsync();


[DependsOn(typeof(AbpAspNetCoreModule))]
[DependsOn(typeof(AbpAutofacModule))] //Add dependency to ABP Autofac module
[DependsOn(typeof(AbpBackgroundWorkersModule))]
public class AppModule : AbpModule
{

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

	protected async override Task DoWorkAsync(
		PeriodicBackgroundWorkerContext workerContext)
	{
		Logger.LogInformation("定时器开始，线程:{thread}, 时间:{Time}",Thread.CurrentThread.ManagedThreadId,DateTime.Now);
	
		Thread.Sleep(3000);
	}
}