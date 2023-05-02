<Query Kind="Statements">
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Authentication</Namespace>
  <Namespace>Microsoft.AspNetCore.Authentication.Cookies</Namespace>
  <Namespace>Microsoft.AspNetCore.Authorization</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	EnvironmentName = Environments.Development
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => {
		options.LoginPath = "/login";
		options.AccessDeniedPath = "/denied";
	});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", [Authorize] () => "Protected Resource");
app.MapGet("/home", async (HttpContext context) =>
{
	var result = "";
	var user = context.User;
	if (user.Identity.IsAuthenticated)
	{
		var username = user.Identity.Name;
		var claims = user.Claims;

		// 打印用户信息
		foreach (var claim in claims)
		{
			result +=(claim.Type + ": " + claim.Value + "\n\r");
		}
	}else{
		result = "用户未认证。";
	}
	
	return result;
});
app.MapGet("/login", async (HttpContext  context) =>
{
	var claims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, "Tim"),
		new Claim(ClaimTypes.Role, "Admin")
	};
	var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
	var principal = new ClaimsPrincipal(identity);
	await context.SignInAsync(principal);
	return new { message = "Logged in" };
});
app.MapGet("/denied", () => "Access Denied");

Process.Start(new ProcessStartInfo
{
	FileName = "http://localhost:5000/swagger/index.html",
	UseShellExecute = true,
});

app.Run();