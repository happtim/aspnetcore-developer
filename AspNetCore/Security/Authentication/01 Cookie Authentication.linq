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

//https://www.tektutorialshub.com/asp-net-core/authentication-in-asp-net-core/

//未经身份验证的请求
//1. 请求到达身份验证中间件。
//2. 身份验证中间件检查请求中是否存在正确的凭据。它将使用默认身份验证处理程序来执行此操作。
//	 它可以是一个 Cookie 处理程序/Jwt 处理程序。
//	 由于它找不到任何凭据，因此它将用户属性设置为匿名用户。
//3. 授权中间件 （UseAuthorization） 检查目标页面是否需要授权.
//3.1 如果否，则允许用户访问该页面
//3.2 如果是，它将调用身份验证处理程序上的 ChallengeAsync()。它将用户重定向到登录页面

//登录
//1. 用户在登录表单中出示ID和密码，然后单击登录按钮
//2. 请求在通过身份验证和授权中间件后命中登录终结点。因此，登录终结点必须具有Allowanonymous修饰器，否则请求将永远不会到达它。
//3. 根据数据库验证用户 ID 和密码
//4. 如果使用 Cookie 身份验证处理程序
//	4.1 创建具有用户声明的用户ClaimsPrincipal
//	4.2 使用 创建加密的 Cookie 并将其添加到当前响应中。HttpContext.SignInAsync
//	4.3 响应返回到浏览器
//	4.4 浏览器存储 Cookie
//5. 如果使用 JWT 持有者身份验证处理程序
//	5.1 使用用户的声明创建用户的 JWT 令牌
//	5.2 JWT 令牌作为响应发送给用户
//	5.3 用户读取令牌并将其存储在本地存储、会话存储甚至 cookie 中
//6.用户现在已通过身份验证

//认证后续请求
//7.用户发出保护页面的请求。
//8.如果您使用的是 cookie 身份验证，则无需执行任何操作。
//	浏览器将自动在每个请求中包含 Cookie。
//	但在 JWT 令牌的情况下，您需要在Header中包含Authorization令牌。
//9.请求到达身份验证中间件。
//	它将使用默认的身份验证处理程序来读取 cookie / JWT 令牌，
//	并构造ClaimsIdentity并使用它更新HttpContext.User属性
//10.授权中间件通过检查属性来查看用户已通过身份验证，并允许访问该属性。
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	EnvironmentName = Environments.Development
});
//认证方案
// 每个Authentication handler 通过AddAuthentication注册到asp。net core 的都认为要给认证方案。
// 如果设置了多个认账方案需要指定默认的认证方案。 CookieAuthenticationDefaults.AuthenticationScheme 代表使用Cookie方式。
// 一个认证方案包括：
// 	唯一名字。标识方案。
// 	Authentication Handler
// 	参数

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
	//The ASP.NET Core uses claims-based authentication. 
	//Claim 是一个用户一个信息，如name，email，role。phoneNumber
	//ClaimsIdentity 是一组Claims集合。可以将驾照类比为ClaimsIdentity。
	//ClaimsPrincipal 是ClaimsIdentity的集合，它其实可以认为asp。net core的用户。
	//一个用户可以有多个ClaimsIdentity，如驾照，护照，身份证。
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