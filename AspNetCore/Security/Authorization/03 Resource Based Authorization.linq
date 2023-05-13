<Query Kind="Statements">
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Authentication</Namespace>
  <Namespace>Microsoft.AspNetCore.Authentication.Cookies</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>Microsoft.AspNetCore.Authorization</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "..\Authentication\UserService"

//https://www.tektutorialshub.com/asp-net-core/resource-based-authorization-in-asp-net-core/

//授权中间件在数据绑定和执行操作方法（或页面处理程序）之前运行。因此，它无法访问操作方法所操作的数据（资源）。
//在决定用户是否可以访问资源之前，我们需要首先加载资源。基于Attribute的授权机制无法处理此类用例。

//例如，我们希望只有文档的创建者能够编辑或删除它。其他用户只能查看该文档。
//在这种情况下，我们需要先加载该文档。然后，我们将检查文档的创建者是否与已登录的用户相同。
//只有在这种情况下，我们才能允许用户访问该文档。

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	EnvironmentName = Environments.Development
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>

	{
		options.LoginPath = "/login";
		options.AccessDeniedPath = "/denied";
	});
	
builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options =>
{

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/home", [Authorize(Policy = "canManageProduct")] async (HttpContext context) =>
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
			result += (claim.Type + ": " + claim.Value + "\n\r");
		}
	}

	return result;
});
app.MapGet("/login", async (HttpContext context) =>
{
	return new { message = "Logging page." };
});

app.MapPost("/login", async (LoginModel login, HttpContext context, IUserService userService) =>
{
	var user = await userService.Authenticate(login.Username, login.Password);

	if (user == null)
		return new { message = $"Invalid Username or Password" };

	var claims = new[]
	{
		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		new Claim(ClaimTypes.Name, user.Username),
		new Claim("UserDefined", "whatever"),
		//new Claim("Employee",""),
	};

	var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
	var principal = new ClaimsPrincipal(identity);
	await context.SignInAsync(principal);
	return new { message = $"Logged in {login.Username}" };
});

app.MapPost("/logout", async (HttpContext context) =>
{
	await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
	return new { message = $"Logged out" };
});

app.MapGet("/denied", () =>
{
	var responseBody = new ContentResult
	{
		StatusCode = (int)HttpStatusCode.Unauthorized,
		Content = "Access Denied"
	};
	return responseBody;
});

Process.Start(new ProcessStartInfo
{
	FileName = "http://localhost:5000/swagger/index.html",
	UseShellExecute = true,
});

app.Run();

record LoginModel(string Username, string Password);