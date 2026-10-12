# openiddict同时实现认证和访问API

OpenID Connect和OAuth的结合非常简洁；您可以通过令牌服务在一次交互中实现用户认证和API访问。

在上一节中，登录过程中的令牌请求仅要求身份资源，即仅要求范围如个人资料和开放ID。在此快速入门中，您将在该请求中添加用于API资源的范围。IdentityServer将回复两个令牌：
- 身份令牌，包含了关于认证过程和会话的信息，并且
- 访问令牌，允许代表登录用户访问API。

## 修改服务端Client配置

配置服务的地方，增加AllowRefreshTokenFlow，允许RefreshTokenFlow。

```csharp
// Register the OpenIddict server components.  
.AddServer(options =>  
{
	// Enable the client credentials and authorization code flow.  
	options.AllowClientCredentialsFlow()  
	.AllowAuthorizationCodeFlow()  
	.AllowRefreshTokenFlow();
};
```

Openiddict 客户端配置权限的地方，增加GrantTypes.RefreshToken，以此增加刷新令牌的功能。

```csharp
await manager.CreateAsync(new OpenIddictApplicationDescriptor  
{  
	ClientId = "web",  
	ClientSecret = "secret",
	Permissions =  
	{  
		...
		  
		Permissions.GrantTypes.AuthorizationCode,  
		Permissions.GrantTypes.RefreshToken,  
		Permissions.ResponseTypes.Code,  
		  
		...
	}
}
```

之前的案例中，我没都没有注册Scope，这节中我们注册api1 scope，并且赋予资源数组

```json
["resource_server_1","web"]
```

通过`RegisterScope`方法中分配resource参数，可以将特定的资源与范围相关联。资源可以是标识符（如标识API的名称或URI）或标识API的操作（如获取数据的权限）。这样，当客户端请求包含此范围的访问令牌时，可以根据资源参数的定义来授予或拒绝访问权限。

其中Scope中的Resource要与clinetid一致，才可以返回对应client的scope。

这一点和IdentityServer不一样。IdentityServer是配置API Resources 实现该功能的。

```csharp
static async Task RegisterScopesAsync(IServiceProvider provider)
{
	var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

	if (await manager.FindByNameAsync("api1") is null)
	{
		await manager.CreateAsync(new OpenIddictScopeDescriptor
		{
			DisplayName = "Dantooine API access",
			DisplayNames =
			{
				[CultureInfo.GetCultureInfo("fr-FR")] = "Accès à l'API de démo"
			},
			Name = "api1",
			Resources =
			{
				//Assign the aud value to the resource parameter.
				"resource_server_1",
				"web"
			}
		});
	}
}
```

## 修改API端

为了适应openiddict 授权方式，我们需要修改下API端的认证客户端方式：

通过验证Audience的方式去校验：

```csharp
builder.Services.AddAuthentication("Bearer")  
.AddJwtBearer(options =>  
{  
	options.Authority = "https://localhost:5001";  
	//options.TokenValidationParameters.ValidateAudience = false;  
	options.Audience = "resource_server_1";  
});
```

使用Socpes申明的范围进行授权api访问。

```csharp
builder.Services.AddAuthorization(options =>  
	options.AddPolicy("ApiScope", policy =>  
	{  
		policy.RequireAuthenticatedUser();  
		policy.RequireClaim("scope", "api1");  
	})  
);  
  
builder.Services.AddScopeTransformation();
```

## 修改客户端

客户端申请的Scope中也要增加对应的offline_access。

```csharp
 .AddOpenIdConnect("oidc", options =>
{
	options.Authority = "https://localhost:5001";

	options.ClientId = "web";
	
	options.ClientSecret = "secret";
	options.ResponseType = "code";


	...
	options.Scope.Add("offline_access");
	options.Scope.Add("api1");
	
	...
});
```

## 使用Access Token

使用Access Token 代码和[[3-authentication-api-access]]一样。


## 保护API几种方式

### 通过JWT保护

在上述案例中，我们只关心确保访问令牌来自您信任的IdentityServer。JWT的配置如下。

```csharp
builder.Services.AddAuthentication("Bearer")  
	.AddJwtBearer(options =>  
	{  
		options.Authority = "https://localhost:5001";  
		options.TokenValidationParameters.ValidateAudience = false;  
		//options.Audience = "resource_server_1";  
	});
```


也可以通过openiddict提供的验证器进行认证。

```csharp
builder.Services.AddOpenIddict()  
	.AddValidation(options =>  
	{  
		// Note: the validation handler uses OpenID Connect discovery  
		// to retrieve the address of the introspection endpoint.  
		options.SetIssuer("https://localhost:5001/");  
		  
		// options.UseIntrospection()  
		// .SetClientId("web")  
		// .SetClientSecret("secret");  
		  
		options.AddAudiences("resource_server_2");  
		  
		// Register the System.Net.Http integration.  
		options.UseSystemNetHttp();  
		  
		// Register the ASP.NET Core host.  
		options.UseAspNetCore();  
	});
```

配置OpenIddict验证服务以使用直接验证。 默认情况下，直接验证使用IdentityModel来验证JWT令牌。

我们也可以使用UseIntrospection 方式，当使用introspection时，将发送一个OAuth 2.0 introspection请求到授权服务器以验证接收到的访问令牌。


### 通过audience验证

验证token时，可以指定audience，当授权服务器为多个不同的资源服务器发行访问令牌时，建议设置此属性。


### 通过scope的授权

访问令牌将包括用于授权的附加声明，例如范围声明将反映客户端在令牌请求期间请求的（并且已被授予的）范围。

在ASP.NET core中，JWT负载的内容会转换为声明，并打包在ClaimsPrincipal中。为了更好地封装和重用，使用ASP.NET Core授权策略功能。使用此方法，首先将声明要求转化为命名策略：

```csharp
builder.Services.AddAuthorization(options =>  
	options.AddPolicy("ApiScope", policy =>  
	{  
		policy.RequireAuthenticatedUser();  
		policy.RequireClaim("scope", "api1");  
	})  
);
```

...然后强制执行它，例如使用路由表：

```csharp
app.MapControllers().RequireAuthorization("ApiScope");
```

### Scope声明格式

openiddict使用OAuth规范中的JWT配置要求scope声明是一个用空格分隔的字符串。但这意味着使用访问令牌的代码可能需要进行调整。

可以安装包：

```
dotnet package add IdentityModel.AspNetCore.AccessTokenValidation
```

在程序配置服务如下：

```csharp
builder.Services.AddScopeTransformation();
```
