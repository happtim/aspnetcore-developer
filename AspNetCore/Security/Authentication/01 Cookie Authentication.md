# ASP.NET Core中使用Cookie身份验证

## 代码示例

在这个教程中，让我们学习如何使用ASP.NET Core中的Cookie身份验证构建一个用户注册/登录和注销表单。Cookie身份验证是Web应用非常常见的一种验证方式。

| 地址 | .Net版本 |
| -------- | -------- |
| [AspNetCore/Security/Authentication](https://github.com/happtim/aspnetcore-developer-roadmap/tree/main/AspNetCore/Security/Authentication)  | .Net6    | 

### AddAuthentication

AddAuthentication方法返回Authentication Builder，我们使用它来注册Authentication处理程序。

例如，以下代码使用AddCookie方法添加Cookie Authentication处理程序。

```csharp
builder.Services.AddAuthentication()  
    .AddCookie();
```

您可以使用AddAuthentication添加多个身份验证处理程序。以下示例同时添加了Cookie身份验证和JwtBearer身份验证处理程序。

```csharp
services.AddAuthentication()
    .AddJwtBearer()
    .AddCookie();
```

当程序运行的时候登录时，会有如下的错误。是因为我们没有指定默认Scheme。

```
System.InvalidOperationException: No authenticationScheme was specified, and there was no DefaultChallengeScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).
```

我们可以有两种方式解决：

- 添加默认的认证方案

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddJwtBearer()
    .AddCookie();
```

- Authorize属性标签指定认证方案

```csharp
app.MapGet("Account/Home", [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] async (HttpContext context) =>
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
```

### Authentication Scheme


Authentication Scheme可以选择负责生成正确声明集的认证处理程序。每个使用AddAuthentication方法注册的身份验证处理程序都成为一个新的身份验证方案。

一个身份验证方案由以下组成：
1. 一个唯一的名称，用于标识身份验证方案
2. 身份验证处理程序
3. 用于配置处理程序特定实例的选项

每个身份验证处理程序都有它们自己的默认值。Cookie Authentication处理程序在CookieAuthenticationDefaults类中定义了所有的默认值。类似地，JwtBearer使用JwtBearerDefaults类。

- Cookie Authentication的默认名称是“Cookies”（CookieAuthenticationDefaults.AuthenticationScheme）
- JwtBearer Authentication handler使用“Bearer”。(JwtBearerDefaults.AuthenticationScheme)

以下代码与上面的代码相同。

```csharp
services.AddAuthentication()
    .AddJwtBearer("Bearer")
    .AddCookie("Cookies");
```

您还可以多次定义特定的处理程序。下面的示例定义了两次Cookie身份验证，但给它们两个不同的名称。因此现在我们有三个身份验证方案。

```csharp
services.AddAuthentication()
    .AddJwtBearer("Bearer")
    .AddCookie("Cookies")
    .AddCookie("Cookies2")
```

### Authentication Handler Options

每个身份验证处理程序都带有一些选项，您可以配置这些选项来对身份验证处理程序进行微调。

如果不做任何AddCookie配置，类似使用默认配置如下：

```csharp
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie("Cookies", options =>
	{
		options.LoginPath = "/Account/Login";
		options.LogoutPath = "/Account/Logout";
		options.AccessDeniedPath = "/Account/AccessDenied";
		options.ReturnUrlParameter = "ReturnUrl";
	});
```

它是在CookieAuthenticationDefaults类中配置的。以下是该类中定义的默认值。

```csharp
public static class CookieAuthenticationDefaults  
{  
    public const string AuthenticationScheme = "Cookies";  
  
    public static readonly string CookiePrefix = ".AspNetCore.";  
  
    public static readonly PathString LoginPath = new PathString("/Account/Login");  
  
    public static readonly PathString LogoutPath = new PathString("/Account/Logout");  
  
    public static readonly PathString AccessDeniedPath = new PathString("/Account/AccessDenied");  
  
    public static readonly string ReturnUrlParameter = "ReturnUrl";  
}
```

配置JWT承载验证处理程序。

```csharp
services.AddAuthentication(x =>
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = true;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtTokenConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtTokenConfig.Audience,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret))
        };
    });   
```


### Authentication Middleware

UseAuthentication()注册身份验证中间件。

身份验证中间件的主要目的是使用ClaimsPrincipal更新HttpContext.User属性。为了实现这一点，它使用默认的身份验证处理程序并调用AuthenticateAsync()方法。正如前面提到的，身份验证处理程序的AuthenticateAsync()方法必须返回ClaimsPrincipal。

我们将身份验证中间件放置在EndPoint Routing之后。因此，它知道处理请求的控制器操作方法是哪个。

它必须位于授权中间件（UseAuthorization()）和终结点中间件（UseEndPoints()）之前。
- 在使用 UseRouting 后，使路由信息能够用于认证决策。
- 在使用 UseEndpoints 之前，使用户在访问端点之前完成身份验证。

在中间件管道中的UseAuthentication()之后出现的所有中间件都可以通过检查HttpContext.User属性来检查用户是否经过身份验证。

```csharp
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapRazorPages();
});
```


### Login

```csharp
app.MapPost("Account/Login", async (LoginModel login ,HttpContext context,IUserService userService) =>
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
```

1. 首先定义了一个路由路径为"Account/Login"的post请求处理程序。
2. 当收到"Account/Login"的post请求时，处理程序会接收三个参数：LoginModel对象、HttpContext对象和IUserService接口对象。
3. 接下来，处理程序会调用userService的Authenticate方法来验证用户的登录信息。
4. 如果返回的user为null，表示登录信息无效，处理程序会返回一个包含错误信息的对象。
5. 如果验证成功，处理程序会创建一个包含用户信息的Claims数组。
6. 然后，使用Claims数组和CookieAuthenticationDefaults.AuthenticationScheme参数创建一个ClaimsIdentity对象。
7. 再使用ClaimsIdentity对象创建一个ClaimsPrincipal对象。
8. 最后，调用context的SignInAsync方法创建一个加密的cookie并添加到response中，实现登录功能，并返回一个包含登录成功信息的对象。

### List Claims

为了检查用户是否登录，我们可以使用用户属性。用户属性实际上是ClaimsPrincipal的一个实例。它会自动注入到控制器中。如果用户是登录的状态则列出用户所包含的Claims。

```csharp
app.MapGet("Account/Home", [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] async (HttpContext context) =>
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
```

### Logout

调用await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)方法进行用户注销操作。该方法会注销当前用户的身份认证。

```csharp
app.MapPost("Account/Logout", async (HttpContext context) => 
{
  await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
  return new { message = $"Logged out" };
});
```


