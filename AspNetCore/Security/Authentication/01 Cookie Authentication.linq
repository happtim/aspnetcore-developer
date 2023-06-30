<Query Kind="Statements">
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Authentication</Namespace>
  <Namespace>Microsoft.AspNetCore.Authentication.Cookies</Namespace>
  <Namespace>Microsoft.AspNetCore.Authorization</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>Microsoft.OpenApi.Models</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load ".\UserService"

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	EnvironmentName = Environments.Development
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/login";
		options.AccessDeniedPath = "/denied";
	})
	;
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService,UserService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//UseAuthentication主要作用就是使用ClaimsPrincipal更新HttpContext.User属性。
//为了实现它，使用调用AuthenticateAsync()返回 ClaimsPrincipal。
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/home", [Authorize] async (HttpContext context) =>
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

app.MapPost("/login", async (LoginModel login ,HttpContext context,IUserService userService) =>
{
	var user = await userService.Authenticate(login.Username,login.Password);
	
	if(user == null)
		return new { message = $"Invalid Username or Password" };
		
	var claims = new[]
	{
		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		new Claim(ClaimTypes.Name, user.Username),
		new Claim("UserDefined", "whatever"),
	};
	var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
	var principal = new ClaimsPrincipal(identity);
	//SignInAsync 创建一个加密的cookie并添加到response中。
	await context.SignInAsync(principal);
	return new { message = $"Logged in {login.Username}" };
});

app.MapPost("/logout", async (HttpContext context) => 
{
  await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
  return new { message = $"Logged out" };
});

Process.Start(new ProcessStartInfo
{
	FileName = "http://localhost:5000/swagger/index.html",
	UseShellExecute = true,
});

app.Run();

record LoginModel(string Username, string Password);