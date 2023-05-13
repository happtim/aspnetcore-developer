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
#load ".\ProductService"

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
	options.AddPolicy("sameAuthorPolicy",
		policy =>
			policy.AddRequirements(
				new SameAuthorRequirement()
			));

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


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

//创建一个产品
app.MapPost("/product",  [Authorize] async (
	HttpContext context,
	 Product product,
	IProductService productService) => {

	var User = context.User;
	product.CreatedUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
	await productService.InsertAsync(product);
	
	return product;
	
});

//编辑一个产品页面
app.MapGet("/edit/{id}", async ( 
	HttpContext context, 
	int? id ,
	IAuthorizationService  authorizationService, 
	IProductService productService) =>
{
	if (id == null)
	{
		return Results.NotFound();
	}

	var product = await productService.FindAsync(id);
	if (product == null)
	{
		return Results.NotFound();
	}
	
	var User = context.User;

	var result = await authorizationService.AuthorizeAsync(User, product, "sameAuthorPolicy");
	if (!result.Succeeded)
	{
		if (User.Identity.IsAuthenticated)
		{
			return Results.Forbid();
		}
		else
		{
			return Results.Challenge();
		}
	}
	
	return Results.Ok(product);

});

Process.Start(new ProcessStartInfo
{
	FileName = "http://localhost:5000/swagger/index.html",
	UseShellExecute = true,
});

app.Run();

record LoginModel(string Username, string Password);

//一个Requirement 类必须从Microsoft.AspNetCore.Authorization命名空间实现IAuthorizationRequirement。
public class SameAuthorRequirement : IAuthorizationRequirement
{
}

//创建Handler 来验证SameAuthorRequirement是否是有效的。
public class DocumentAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, Product>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
													SameAuthorRequirement requirement,
													Product resource)
	{

		//在代码中，我们检查产品（资源）的CreatedUserID是否与已登录用户的相同。如果为真则返回成功。
		if (context.User.HasClaim(ClaimTypes.NameIdentifier, resource.CreatedUserID))
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

