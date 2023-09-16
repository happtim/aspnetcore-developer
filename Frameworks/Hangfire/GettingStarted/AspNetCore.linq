<Query Kind="Statements">
  <NuGetReference Version="1.8.5">Hangfire.AspNetCore</NuGetReference>
  <NuGetReference>Hangfire.InMemory</NuGetReference>
  <Namespace>Hangfire</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

var builder = WebApplication.CreateBuilder();

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
	.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
	.UseSimpleAssemblyNameTypeSerializer()
	.UseRecommendedSerializerSettings()
	.UseInMemoryStorage());

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

var app = builder.Build();

var scopeFactory = app.Services.GetService<IServiceScopeFactory>();

using (var scope = scopeFactory.CreateScope()) 
{
	var backgroundJobs = scope.ServiceProvider.GetService<IBackgroundJobClient>();

	backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
	endpoints.MapHangfireDashboard();
});

app.Run();