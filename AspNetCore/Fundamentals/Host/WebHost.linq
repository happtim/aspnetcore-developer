<Query Kind="Statements">
  <Namespace>Microsoft.AspNetCore</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


//https://zhuanlan.zhihu.com/p/438182496
//Asp.net Core 2.x 的时候，Asp.net Core通过webhost进行应用的配置，这个时候框架中已经应用了概念：Host和Application,
//也即将整个应用分成了两大部分，一个部分用户基础框架，基础组件的配置 - Host的配置，而应用的配置则主要用于客户应用的配置，
//例如向依赖管理容器中添加用户的service, 添加处理请求的中间件，配置Endpoint等等，在这两个概念下，
//也自然而然的将整个应用的配置分配到了两个文件中，这个就是经典的Program.cs和Startup.cs

WebHost.CreateDefaultBuilder()
	.ConfigureAppConfiguration((hostingContext, config) =>
	{
		config.AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true);
	})
	.ConfigureLogging(logging =>
	{
		logging.SetMinimumLevel(LogLevel.Trace);
	})
.UseStartup<Startup>()
.UseUrls("http://*:5000;https://localhost:5001")
.Build()
.Run();

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
	public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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