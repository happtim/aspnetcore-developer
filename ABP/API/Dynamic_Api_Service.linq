<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.AspNetCore</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Http.Client</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Swashbuckle</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.OpenApi.Models</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.AspNetCore</Namespace>
  <Namespace>Volo.Abp.AspNetCore.Mvc</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.DependencyInjection</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Volo.Abp.Swashbuckle</Namespace>
  <Namespace>Volo.Abp.Http.Client</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load ".\Contracts"

var builder = WebApplication.CreateBuilder();

builder.Host.UseAutofac();

await builder.AddApplicationAsync<AppModule>();

var app = builder.Build();

await app.InitializeApplicationAsync();

await app.RunAsync();

[DependsOn(typeof(AbpAspNetCoreMvcModule))]
[DependsOn(typeof(AbpHttpClientModule))]
[DependsOn(typeof(AbpSwashbuckleModule))]
[DependsOn(typeof(AbpAutofacModule))] //Add dependency to ABP Autofac module
public class AppModule : AbpModule
{
	public override void ConfigureServices(ServiceConfigurationContext context)
	{ 
		var services = context.Services;
		
		services.AddAbpSwaggerGen(
			options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });
				options.DocInclusionPredicate((docName, description) => true);
				options.CustomSchemaIds(type => type.FullName);
			}
		);
		
		//services.AddControllers();

//		Configure<AbpAspNetCoreMvcOptions>(options =>
//	   {
//		   options
//			   .ConventionalControllers
//			   .Create(typeof(AppModule).Assembly);
//	   });

		//Create dynamic client proxies
		context.Services.AddHttpClientProxies(
			typeof(AppModule).Assembly
		);

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

		app.UseSwagger();
		app.UseAbpSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");
		});

		//app.UseEndpoints(options => 
		//{
		//	options.MapControllers();
		//});

		app.UseConfiguredEndpoints();
	}
}

[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : AbpControllerBase, IHelloWorldService
{
	[HttpGet]
	[Route("call-hello")]
	public Task<string> GetCallHelloAsync()
	{
		return Task.FromResult("Hello Wrold!");
	}
}