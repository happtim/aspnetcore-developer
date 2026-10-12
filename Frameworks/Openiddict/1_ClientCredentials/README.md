# openiddict实现Client Credential

类似identityserver提供的clinet credential 授权方式，openiddict也能实现类似的功能。
项目分为3部分，服务端，客户端，还有api资源端。

## 创建一个Openiddict服务端


### 配置数据库

openiddict使用是依赖于数据库的，所以我们先添加数据服务。我们使用轻量的sqlite作为数据存储源。

```csharp
services.AddDbContext<ApplicationDbContext>(options =>  
{  
	// Configure the context to use sqlite.  
	options.UseSqlite($"Filename=openiddict-aridka-server.db");  
	  
	// Register the entity sets needed by OpenIddict.  
	// Note: use the generic overload if you need  
	// to replace the default OpenIddict entities.  
	options.UseOpenIddict();  
});
```

### 配置Openiddict

添加openiddct服务，需要配置AddOpenIddict，其余添加Core，和服务器。

```csharp
services.AddOpenIddict()  
	  
	// Register the OpenIddict core components.  
	.AddCore(options =>  
	{  
		// Configure OpenIddict to use the Entity Framework Core stores and models.  
		// Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.  
		options.UseEntityFrameworkCore()  
		.UseDbContext<ApplicationDbContext>();  
	  
	})  
	  
	// Register the OpenIddict server components.  
	.AddServer(options =>  
	{  
		// Enable the token endpoint.  
		options  
		.SetTokenEndpointUris("connect/token");  
		  
		// Enable the client credentials flow.  
		options.AllowClientCredentialsFlow();  
		  
		// Register the signing and encryption credentials.  
		options  
		.AddEphemeralEncryptionKey()  
		.AddEphemeralSigningKey()  
		.DisableAccessTokenEncryption();  
	
		// Register scopes (permissions)  
		options.RegisterScopes("api1");
		  
		// Register the ASP.NET Core host and configure the ASP.NET Core-specific options.  
		options.UseAspNetCore()  
		.EnableTokenEndpointPassthrough();  
	});
```

openIddict需要手动配置endpoint。在这个案例中我们只使用token endpoint用于交换access_token。以及用于交换令牌的controller也是自己来维护。这点比identityserver复杂一些。

然后通过`.AllowClientCredentialsFlow()`方法启用了客户端凭证流程，openiddict的授权流程也是需要手动指定的，如果不加指定，是不会出现在https://localhost:5001/.well-known/openid-configuration列出的。 

接着，通过`.AddEphemeralEncryptionKey()`和`.AddEphemeralSigningKey()`方法注册了签名和加密凭证。这里使用了临时凭证，但在实际应用中应该使用具有长期有效性的凭证。同时我们禁用了access_token的加密功能，这样更易于我们观察access_token。

RegisterScopes定义了所支持的范围（权限）。在这种情况下，我们有一个名为api1的范围，但是授权服务器可以支持多个范围。

最后，通过`.UseAspNetCore()`方法注册了ASP.NET Core主机。通过`.EnableTokenEndpointPassthrough()`方法启用了令牌终结点的穿透模式，以便可以在ASP.NET Core应用程序中自定义令牌终结点的行为。

要检查OpenIddict是否配置正确，我们可以启动应用程序并导航到：https://localhost:5001/.well-known/openid-configuration，响应应该是这样的：

```json
{
  "issuer": "https://localhost:5001/",
  "token_endpoint": "https://localhost:5001/connect/token",
  "jwks_uri": "https://localhost:5001/.well-known/jwks",
  "grant_types_supported": [
    "client_credentials"
  ],
  "scopes_supported": [
    "openid",
    "api1"
  ],
  "claims_supported": [
    "aud",
    "exp",
    "iat",
    "iss",
    "sub"
  ],
  "id_token_signing_alg_values_supported": [
    "RS256"
  ],
  "subject_types_supported": [
    "public"
  ],
  "token_endpoint_auth_methods_supported": [
    "client_secret_basic",
    "client_secret_post"
  ],
  "claims_parameter_supported": false,
  "request_parameter_supported": false,
  "request_uri_parameter_supported": false,
  "authorization_response_iss_parameter_supported": true
}
```

### 配置Client

openIddict使用数据库存储配置，在程序启动时候初始化数据库，客户端信息如下：

```csharp
static async Task RegisterApplicationsAsync(IServiceProvider provider)
{
	var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

	// API
	if (await manager.FindByClientIdAsync("client") == null)
	{
		await manager.CreateAsync(new OpenIddictApplicationDescriptor
		{
			ClientId = "client",
			ClientSecret = "secret",
			DisplayName = "My client application",
			Permissions =
			{
				Permissions.Endpoints.Token,
				Permissions.GrantTypes.ClientCredentials,
				Permissions.Prefixes.Scope + "api1"
			}
		});
	}
}
```

其中配置了客户端id，客户端密码，以及客户拥有的权限。在本例中，我们允许客户端使用客户端凭据流，访问令牌终点，并允许客户端请求api1范围。

### 创建Token Endpoint

代码创建了一个ClaimsIdentity对象，用于存储生成令牌时需要持久化的声明信息。之后，代码将client_id和应用程序的显示名称添加到ClaimsIdentity的声明中。

目前，我们需要专注于客户端凭据流程。当请求进入交换操作时，客户端凭据（ClientId 和 ClientSecret）已经由 OpenIddict 验证通过。因此，我们不需要对请求进行身份验证，我们只需创建一个声明主体并使用它进行登录。

接下来，我们还通过调用claimsPrincipal.SetScopes(request.GetScopes());来授予所有请求的作用域。OpenIddict已经检查了请求的作用域是否被允许（一般来说，还有当前客户端）。我们在这里手动添加作用域的原因是，如果需要，我们可以过滤在这里授予的作用域。

最后，使用生成的ClaimsPrincipal对象调用SignIn方法，我们必须基于OpenIddictServerAspNetCoreDefaults.AuthenticationScheme创建声明主体。这样，在我们在方法结束时调用SignIn时，OpenIddict中间件将处理登录并将访问令牌作为响应返回给客户端。

```csharp
[HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
public async Task<IActionResult> Exchange()
{
	var request = HttpContext.GetOpenIddictServerRequest();
	if (request.IsClientCredentialsGrantType())
	{
		// Note: the client credentials are automatically validated by OpenIddict:
		// if client_id or client_secret are invalid, this action won't be invoked.

		var application = await _applicationManager.FindByClientIdAsync(request.ClientId);
		if (application == null)
		{
			throw new InvalidOperationException("The application details cannot be found in the database.");
		}

		// Create the claims-based identity that will be used by OpenIddict to generate tokens.
		var identity = new ClaimsIdentity(
			authenticationType: TokenValidationParameters.DefaultAuthenticationType,
			nameType: Claims.Name,
			roleType: Claims.Role);

		// Add the claims that will be persisted in the tokens (use the client_id as the subject identifier).
		identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
		identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

		// Note: In the original OAuth 2.0 specification, the client credentials grant
		// doesn't return an identity token, which is an OpenID Connect concept.
		//
		// As a non-standardized extension, OpenIddict allows returning an id_token
		// to convey information about the client application when the "openid" scope
		// is granted (i.e specified when calling principal.SetScopes()). When the "openid"
		// scope is not explicitly set, no identity token is returned to the client application.

		// Set the list of scopes granted to the client application in access_token.
		identity.SetScopes(request.GetScopes());
		
		//identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
		identity.SetDestinations(GetDestinations);

		return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
	}

	throw new NotImplementedException("The specified grant type is not implemented.");
}
```


## 创建API

### 创建Bearer验证方式方式API

创建Bearer验证方式的和identityserver一样。参见：[[content/dotnet/aspnetcore/security/identityserver/quickstart/1-client-credentials|1-client-credentials]]

### 创建openIddict验证方式API

该案例没有使用`Microsoft.AspNetCore.Authentication.JwtBearer`进行验证jwt。参数 options.SetIssuer("https://localhost:5001/"); 用来验证令牌颁发者。没有配置AddAudiences则不验证受众。

```csharp
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
     // Note: the validation handler uses OpenID Connect discovery
     // to retrieve the address of the introspection endpoint.
     options.SetIssuer("https://localhost:5001/");
     //options.AddAudiences("resource_server_1");

     // Register the System.Net.Http integration.
     options.UseSystemNetHttp();

     // Register the ASP.NET Core host.
     options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
```


## 创建客户端

创建客户端的方式和identityserver一样。参见：[[content/dotnet/aspnetcore/security/identityserver/quickstart/1-client-credentials|1-client-credentials]]

## 保护API的几种方式

在上述案例中，我们只关心确保访问令牌来自您信任的OP。

仅仅确保令牌来自受信任的发行者对于大多数情况来说并不足够。在更复杂的系统中，您将拥有多个资源和多个客户端。并非每个客户端都有权限访问每个资源。

在OAuth中，有两种互补的机制可以嵌入有关令牌的“功能性”更多信息 - audience和scope。

注册audience如下：

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
				"resource_server_1"
			}
		});
	}
}
```

Resources这个参数其实是Audience意思。代表api1的scope是只针对资源1服务器的。当客户端获取到access_token之后，在调用api资源的时候，资源端是可以验证这个access_token是否能访问本资源服务api。

