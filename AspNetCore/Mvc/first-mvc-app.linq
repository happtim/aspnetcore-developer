<Query Kind="Statements">
  <NuGetReference Version="6.0.16">Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	EnvironmentName = Environments.Development
});

builder.Services.AddMvc().AddRazorRuntimeCompilation(sa =>
{
	sa.FileProviders.Add(new PhysicalFileProvider(Path.GetDirectoryName(Util.CurrentQueryPath)));
	
	Util.Framework.GetReferenceAssemblies(true)
		.Prepend(GetType().Assembly.Location)
		.ToList()
		.ForEach(sa.AdditionalReferencePaths.Add);
})
//添加本程序内的Controllers
.AddApplicationPart(this.GetType().Assembly);

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(
		Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "wwwroot")),
});

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();


public class HomeController : Controller
{
	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}
}

public class HelloWorldController : Controller 
{

	// GET: /HelloWorld/
	public IActionResult  Index()
	{
		return View();
	}
	// 
	// GET: /HelloWorld/Welcome/ 
	public string Welcome()
	{
		return "This is the Welcome action method...";
	}
}