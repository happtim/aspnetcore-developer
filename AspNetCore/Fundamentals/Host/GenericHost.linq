<Query Kind="Statements">
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Hosting</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


//Asp.net Core 3.x/5.x 带来最大的改变就是将上个版本的WebHost中的Host功能改为了GenericHost, 
//使得Asp.net Core的框架不再仅仅是为Web应用准备的，而是通过GenericHost共享了很多基础的代码给其他的应用，
//例如BackgroundService， gRPC Service, Windows Service等等。

Host.CreateDefaultBuilder()
 	.ConfigureServices(services =>
	{
		services.AddHostedService<Worker>();
	})
	.ConfigureWebHostDefaults(webBuilder => 
	{
		webBuilder.UseStartup<Startup>();
	})
  .Build().Run();

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddMvc(options => options.EnableEndpointRouting = false);
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseStaticFiles();
		app.UseMvc(routes =>
	   {
		   routes.MapRoute(
			   name: "default",
			   template: "{controller=Home}/{action=Index}/{id?}");
	   });
	}
}

public sealed class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;

	public Worker(ILogger<Worker> logger) =>
		_logger = logger;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
			await Task.Delay(1_000, stoppingToken);
		}
	}
}