# ASP.NET Core中使用Basic身份验证

## 什么是Basic身份认证？

Basic身份认证（Basic Authentication）是一种在网页、网络应用或API等环境中使用的身份验证机制。在Basic身份认证中，客户端在发送请求时，将用户名和密码组合成一个字符串，并进行Base64编码。然后，将该编码字符串添加到HTTP请求的Authorization头部中作为身份凭证，格式为"Basic 编码字符串"。

服务端接收到请求后，会从Authorization头部中提取出编码字符串，并进行解码，得到用户名和密码。然后，服务端会将得到的用户名和密码与存储在服务器上的用户凭证进行对比。如果验证成功，服务端会返回请求的资源；否则，返回401状态码表示身份验证失败。

Basic身份认证是一种简单而常见的身份验证机制，但由于其将凭证以Base64编码的形式传输，容易被截获并解码，因此不适合用于保护敏感信息的场景。一般来说，建议在实际应用中使用更安全的身份认证机制，如OAuth、JWT等。

### 协议示例

```
GET /api/resource HTTP/1.1
Host: example.com
Authorization: Basic dXNlcm5hbWU6cGFzc3dvcmQ=
```

在上述示例中，客户端将用户名"username"和密码"password"按照`username:password`进行Base64编码得到"dXNlcm5hbWU6cGFzc3dvcmQ="，并将该编码字符串添加到请求的Authorization头部中。

## 代码示例

| 地址 | .Net版本 |
| -------- | -------- |
| [AspNetCore/Security/Authentication](https://github.com/happtim/aspnetcore-developer-roadmap/tree/main/AspNetCore/Security/Authentication)  | .Net6    | 

### AddAuthentication

```csharp
builder.Services
	.AddAuthentication("BasicAuthentication")
	.AddCookie(s => {})
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
```

上述代码将"BasicAuthentication"作为默认身份验证方案，添加了Cookie身份验证方案，并添加了一个自定义的身份验证方案"BasicAuthentication"，该方案可以使用`BasicAuthenticationHandler`进行处理。

通过调用`.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null)`方法，向身份验证服务添加一个自定义的身份验证方案。该方法有两个泛型类型参数，分别是用于配置选项的类型（在这里为`AuthenticationSchemeOptions`）和用于实现身份验证处理的处理程序的类型（在这里为`BasicAuthenticationHandler`）。通过第一个参数指定方案的名称为"BasicAuthentication"，第二个参数为`null`表示不提供选项配置。

其实我们熟悉的`.AddCookie(s => {})` 方法，最终也是调用 `builder.AddScheme`添加到Scheme中的

```csharp
public static AuthenticationBuilder AddCookie(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<CookieAuthenticationOptions> configureOptions)  
{  
	builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureCookieAuthenticationOptions>());  
	builder.Services.AddOptions<CookieAuthenticationOptions>(authenticationScheme).Validate((CookieAuthenticationOptions o) => !o.Cookie.Expiration.HasValue, "Cookie.Expiration is ignored, use ExpireTimeSpan instead.");  
	return builder.AddScheme<CookieAuthenticationOptions, CookieAuthenticationHandler>(authenticationScheme, displayName, configureOptions);  
}
```
### Authentication Handlers

认证处理程序负责以下工作：

1. 认证用户（使用AuthenticateAsync()方法）。
2. 如果用户未经过认证，则将用户重定向到登录页（使用ChallengeAsync()方法挑战当前请求）。
3. 如果用户已经经过认证，但没有授权，则禁止当前请求（使用ForbidAsync()方法）。

认证处理程序必须实现IAuthenticationHandler接口。该接口包含三个方法：AuthenticateAsync()、ChallengeAsync()和ForbidAsync()。

Authentication Handlers 的 AuthenticateAsync() 方法负责从请求中构建 ClaimsPrincipal 并将其返回给身份验证中间件。然后，身份验证中间件使用 ClaimsPrincipal 设置 HttpContext.User 属性。

例如，cookie 身份验证处理程序的 AuthenticateAsync() 方法必须从当前请求中读取 cookie、构建 ClaimsPrincipal 并返回它。类似地，JWT 承载处理程序必须对 JWT 承载令牌进行反序列化和验证，并构建 ClaimsPrincipal 并返回它。

```csharp

protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
{
	if (!Request.Headers.ContainsKey("Authorization"))
	{
		return AuthenticateResult.Fail("Missing Authorization header");
	}

	User user = null;
	try
	{
		var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
		var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
		var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
		var username = credentials[0];
		var password = credentials[1];
		user = await _userService.Authenticate(username,password);
	   
	}
	catch
	{
		return AuthenticateResult.Fail("Invalid Authorization header");
	}
	
	if (user == null)
		return AuthenticateResult.Fail("Invalid Username or Password");

	var claims = new[]
	{
		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		new Claim(ClaimTypes.Name, user.Username),
		new Claim("UserDefined", "whatever"),
	};
	var identity = new ClaimsIdentity(claims, Scheme.Name);
	var principal = new ClaimsPrincipal(identity);
	var ticket = new AuthenticationTicket(principal, Scheme.Name);
	return AuthenticateResult.Success(ticket);
}

```


首先使用AuthenticationHeaderValue.Parse方法从HTTP请求头的Authorization字段中解析出授权信息，并将结果存储在authHeader变量中。

接下来，从authHeader变量中获取授权信息的Parameter字段，并使用Convert.FromBase64String方法将其解码为字节数组，然后将结果存储在credentialBytes变量中。

然后，使用Encoding.UTF8.GetString方法将credentialBytes中的字节数组转换为字符串，并使用Split方法以冒号为分隔符，将字符串分割为两部分，分别存储在credentials数组的第一个和第二个元素中。这个数组中第一个元素是用户名，第二个元素是密码。

调用_userService.Authenticate方法，使用解析到的用户名和密码进行用户认证，并将认证结果存储在user变量中。如果认证失败，会抛出异常。

如何验证通过，创建一个声明数组`claims`，其中包含了用户身份的不同属性。每个声明都是一个`Claim`对象。这里示例中的`claims`数组包含了`name identifier`，`name`和`user defined`三个声明。

使用`claims`数组创建一个`ClaimsIdentity`对象`identity`，指定了该身份验证的所用方案名称。

使用`identity`对象创建一个`ClaimsPrincipal`对象`principal`，该对象代表了当前用户的主体。

使用`principal`对象创建一个`AuthenticationTicket`对象`ticket`，该对象表示验证成功的结果，并包含了`principal`。


## Microsoft.AspNetCore.Authentication 代码解释

### UseAuthentication （ AuthenticationMiddleware） 

UseAuthentication 使用中间件为 AuthenticationMiddleware。

```csharp
public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app)
{
	if (app == null)
	{
		throw new ArgumentNullException("app");
	}
	return app.UseMiddleware<AuthenticationMiddleware>(Array.Empty<object>());
}
```

下面代码为`AuthenticationMiddleware`的Invoke函数， `AuthenticateResult authenticateResult = await context.AuthenticateAsync(authenticationScheme.Name); ` 就是通过注册Scheme寻找到对应Handler然后返回`Principal`。

```csharp
public async Task Invoke(HttpContext context)  
{  
	context.Features.Set((IAuthenticationFeature?)new AuthenticationFeature  
	{  
		OriginalPath = context.Request.Path,  
		OriginalPathBase = context.Request.PathBase  
	});  
	IAuthenticationHandlerProvider handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();  
	foreach (AuthenticationScheme item in await Schemes.GetRequestHandlerSchemesAsync())  
	{  
		IAuthenticationRequestHandler authenticationRequestHandler = (await handlers.GetHandlerAsync(context, item.Name)) as IAuthenticationRequestHandler;  
		bool flag = authenticationRequestHandler != null;  
		if (flag)  
		{  
			flag = await authenticationRequestHandler.HandleRequestAsync();  
		}  
		if (flag)  
		{  
			return;  
		}  
	}  
	AuthenticationScheme authenticationScheme = await Schemes.GetDefaultAuthenticateSchemeAsync();  
	if (authenticationScheme != null)  
	{  
		AuthenticateResult authenticateResult = await context.AuthenticateAsync(authenticationScheme.Name);  
		if (authenticateResult?.Principal != null)  
		{  
			context.User = authenticateResult.Principal;  
		}  
		if (authenticateResult != null && authenticateResult.Succeeded)  
		{  
			AuthenticationFeatures instance = new AuthenticationFeatures(authenticateResult);  
			context.Features.Set((IHttpAuthenticationFeature?)instance);  
			context.Features.Set((IAuthenticateResultFeature?)instance);  
		}  
	}  
	await _next(context);  
}
```

### AuthenticationService

`await context.AuthenticateAsync(authenticationScheme.Name);` 方法实际是一个扩展方法，内部调用的`IAuthenticationService`。

```csharp
public static Task<AuthenticateResult> AuthenticateAsync(this HttpContext context, string? scheme)
{
	return context.RequestServices.GetRequiredService<IAuthenticationService>().AuthenticateAsync(context, scheme);
}
```

下面核心代码。 调用`Handlers.GetHandlerAsync`方法来异步获取用于指定身份验证方案（scheme）的身份验证处理程序（authentication handler）。

```csharp
public virtual async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
{
	if (scheme == null)
	{
		scheme = (await Schemes.GetDefaultAuthenticateSchemeAsync())?.Name;
		if (scheme == null)
		{
			throw new InvalidOperationException("No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
		}
	}
	IAuthenticationHandler authenticationHandler = await Handlers.GetHandlerAsync(context, scheme);
	if (authenticationHandler == null)
	{
		throw await CreateMissingHandlerException(scheme);
	}
	AuthenticateResult result = (await authenticationHandler.AuthenticateAsync()) ?? AuthenticateResult.NoResult();
	if (result.Succeeded)
	{
		ClaimsPrincipal claimsPrincipal = result.Principal;
		bool flag = true;
		if (_transformCache == null)
		{
			_transformCache = new HashSet<ClaimsPrincipal>();
		}
		if (_transformCache.Contains(claimsPrincipal))
		{
			flag = false;
		}
		if (flag)
		{
			claimsPrincipal = await Transform.TransformAsync(claimsPrincipal);
			_transformCache.Add(claimsPrincipal);
		}
		return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, result.Properties, result.Ticket!.AuthenticationScheme));
	}
	return result;
}
```