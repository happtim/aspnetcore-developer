<Query Kind="Statements">
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Authentication</Namespace>
  <Namespace>Microsoft.AspNetCore.Authentication.Cookies</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.AspNetCore.Authorization</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>System.Net</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "..\Authentication\UserService"

//https://www.tektutorialshub.com/asp-net-core/claims-based-authorization-in-asp-net-core/

//基于声明的授权是通过检查用户是否拥有访问URL的声明来实现的。
//在ASP.NET Core中，我们创建策略来实现基于声明的授权。策略定义了用户必须具备哪些声明才能满足策略。
//我们将策略应用于控制器、操作方法、Razor页等。只有那些满足策略的声明的用户才能访问资源，其他人将被重定向到访问被拒绝的页面。

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
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
	//Policy with Claim & Value
	//必需满足Claim Name 为Permission。且值为IT
	//options.AddPolicy("ITOnly", policy => policy.RequireClaim("Permission", "IT"));

	//Policy with Multiple Claims
	//必需同时满足Claim Name 为Permission，且值为IT。和Claim Name 为IT
	//options.AddPolicy("SuperIT",
	//	policy => policy.RequireClaim("Permission", "IT")
	//					.RequireClaim("IT"));
	
	//Policy only for Authorized Users
	//该策略相同与Authorize 属性
	options.AddPolicy("AuthUsers", policy => policy.RequireAuthenticatedUser());
	
	//Policy using a Role
	options.AddPolicy("AdminRole", policy => policy.RequireRole("AdminRole"));
	
	//Policy using a User Name
	options.AddPolicy("TimOnly", policy => policy.RequireUserName("tim"));

	//Custom Policy using a Func
	//之前的策略都很直接简单，如何需要复杂的判断可以使用RequireAssertion方法。
	//options.AddPolicy(
	//	"SuperUser",
	//	policyBuilder => policyBuilder.RequireAssertion(
	//		context => context.User.HasClaim(claim => claim.Type == "Admin")
	//			|| context.User.HasClaim(claim => claim.Type == "IT")
	//			|| context.User.IsInRole("CEO"))
	//);


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
app.MapGet("/home", [Authorize(Policy = "AdminOnly")] async (HttpContext context) =>
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
	};
	
	if(user.Username == "tim"){
		claims = claims.Append(new Claim("Admin","")).ToArray();
	}
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