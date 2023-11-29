<Query Kind="Statements">
  <NuGetReference Version="6.0.3">Volo.Abp.AspNetCore.Mvc</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Autofac</NuGetReference>
  <NuGetReference Version="6.0.3">Volo.Abp.Swashbuckle</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Volo.Abp</Namespace>
  <Namespace>Volo.Abp.AspNetCore</Namespace>
  <Namespace>Volo.Abp.AspNetCore.Mvc</Namespace>
  <Namespace>Volo.Abp.Autofac</Namespace>
  <Namespace>Volo.Abp.Modularity</Namespace>
  <Namespace>Microsoft.OpenApi.Models</Namespace>
  <Namespace>Volo.Abp.Swashbuckle</Namespace>
  <Namespace>Volo.Abp.AspNetCore.Mvc.AntiForgery</Namespace>
  <Namespace>Volo.Abp.Timing</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


var localNow = DateTime.Now;
localNow.Dump(localNow.Kind.ToString());

var utcNow = DateTime.UtcNow;
utcNow.Dump(utcNow.Kind.ToString());

var newDate = new DateTime(2008,8,8);
newDate.Dump(newDate.Kind.ToString());


//local => utc
localNow.ToString("O").Dump();
DateTime.Parse(localNow.ToString("O"),null,DateTimeStyles.AdjustToUniversal).Dump();

//utc => utc
utcNow.ToString("O").Dump();
DateTime.Parse(utcNow.ToString("O"),null,DateTimeStyles.AdjustToUniversal).Dump();

//unspecified => utc
newDate.ToString("O").Dump();
DateTime.Parse(newDate.ToString("O"),null,DateTimeStyles.AdjustToUniversal).Dump();


var builder = WebApplication.CreateBuilder();

builder.Host.UseAutofac();

await builder.AddApplicationAsync<AppModule>();

var app = builder.Build();

await app.InitializeApplicationAsync();

await app.RunAsync();

[DependsOn(typeof(AbpSwashbuckleModule))]
[DependsOn(typeof(AbpAspNetCoreModule))]
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
		
		Configure<AbpAspNetCoreMvcOptions>(options =>
		{
			options
				.ConventionalControllers
				.Create(typeof(AppModule).Assembly);
		});
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
		
		app.UseConfiguredEndpoints();
	}
}

//protected virtual AbpDateTimeModelBinder CreateAbpDateTimeModelBinder(ModelBinderProviderContext context)
//{
//	const DateTimeStyles supportedStyles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal;
//	var dateTimeModelBinder = new DateTimeModelBinder(supportedStyles, context.Services.GetRequiredService<ILoggerFactory>());
//	return new AbpDateTimeModelBinder(context.Services.GetRequiredService<IClock>(), dateTimeModelBinder);
//}

[IgnoreAntiforgeryToken]
[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : AbpControllerBase
{
	private readonly IClock _clock;
	public HelloWorldController(IClock clock)
	{
		_clock = clock;
	}
	[HttpPost]
	public Task<string> PostDateTime( DateTime date)
	{
		DateTime date2 = DateTime.Parse("2023-11-13T09:58:36.0001262+08:00",null,DateTimeStyles.AdjustToUniversal);
		(date2.ToString("O") + " " + date2.Kind.ToString()).Dump();
		date2 = _clock.Normalize(date2);
		(date2.ToString("O") + " " + date2.Kind.ToString()).Dump();
		
		return Task.FromResult(date.ToString("O") + " " + date.Kind.ToString());
	}
}