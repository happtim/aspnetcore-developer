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

//https://www.tektutorialshub.com/asp-net-core/policy-based-authorization-in-asp-net-core/#custom-policy-using-requirement-handlers

//Authorization Requirement
//Authorization Requirement 定义了策略必须评估的条件集合。为使策略成功，必须满足所有需求。
//它类似于AND条件。如果其中一个需求失败，则策略失败。

//Authorization Handler
//Authorization Handler 程序包含用于检查 Requirement 是否有效的逻辑。Requirement 可以包含多个授权处理程序。
//Authorization Handler 可以返回三个值：
// Fail
// Successed
// Do Nothings.

//如果所有的Requirement处理器都没有失败，但至少有一个Requirement处理器返回成功，则Requirement是成功的。
//如果没有任何需求处理程序返回值，则该需求失败。


//示例 如下：
//要在门户中创建一个新产品，用户必须满足以下条件。
//1. 该用户是组织的员工。拥有Employee claim。
//2. 该用户是VIP客户。持有VIP claim。
//3. 用户由于差评而被禁用，他将无法发帖。禁用的账户会有“Disabled”的 Claim。
// 以上条件可以总结一个canManageProduct 策略。
//
//IsEmployeeHandler
//IsVIPCustomerHandler
//IsAccountNotDisabledHandler

//实际上，我们可以创建一个单一的处理程序并测试所有上述内容。
//但在现实世界的应用程序中，可能一个Requirement中重用其他的Handler程序。因此，将其拆分为更小的部分是有意义的。
//(IsEmployeeHandler or  IsVIPCustomerHandler ) and  IsAccountNotDisabledHandler 结果为true才可以访问页面。

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
	options.AddPolicy("canManageProduct",
		   policyBuilder =>
			   policyBuilder.AddRequirements(
				   new IsAccountEnabledRequirement(),
				   new IsAllowedToManageProductRequirement()
			   ));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IAuthorizationHandler, IsAccountNotDisabledHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsEmployeeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsVIPCustomerHandler>();

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

//一个Requirement 类必须从Microsoft.AspNetCore.Authorization命名空间实现IAuthorizationRequirement。
public class IsAccountEnabledRequirement : IAuthorizationRequirement
{
}

public class IsAllowedToManageProductRequirement : IAuthorizationRequirement
{
}

//Handlers 必须继承AuthorizationHandler
public class IsEmployeeHandler : AuthorizationHandler<IsAllowedToManageProductRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
												   IsAllowedToManageProductRequirement requirement)
	{
		//如何是Employee返回true，如果不是nothing
		if (context.User.HasClaim(f => f.Type == "Employee"))
		{
			context.Succeed(requirement);
		}
		return Task.CompletedTask;
	}
}

public class IsVIPCustomerHandler : AuthorizationHandler<IsAllowedToManageProductRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
												   IsAllowedToManageProductRequirement requirement)
	{
		//如何是Vip返回true，如果不是nothing
		if (context.User.HasClaim(f => f.Type == "VIP"))
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class IsAccountNotDisabledHandler : AuthorizationHandler<IsAccountEnabledRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
												   IsAccountEnabledRequirement requirement)
	{
		//如何是Disabled返回false，如果没有返回true
		if (context.User.HasClaim(f => f.Type == "Disabled"))
		{
			context.Fail();
		}
		else
		{
			context.Succeed(requirement);
		}
		return Task.CompletedTask;
	}
}