# ASP.NET Core中使用JWT身份验证

## What is JWT？

JWT是JSON Web Token的缩写，是一种用于进行身份验证和授权的标准方法。JWT由三部分组成：头部(header)、载荷(payload)和签名(signature)。头部包含了算法类型和令牌类型等信息。载荷包含了一些用户相关的信息，如用户ID、角色等。签名用于验证令牌的真实性和完整性。需要注意的是。

**对于已签名的Token，这些信息虽然受到保护，不会被篡改，但任何人都可以阅读。JWT并不加密，签名只负责验证令牌的真实性，不能依靠签名来保护载荷中的信息**

JWT具有以下特点：

1. 可拓展性：JWT支持添加自定义的声明，可以包含任意数量的用户相关信息。
2. 无状态性：服务器不需要保存会话信息，所有授权相关的信息都包含在JWT令牌中。
3. 安全性：JWT使用签名进行验证，防止被篡改，同时可以加密JWT令牌的内容以保护用户隐私。
4. 跨平台和跨语言：JWT基于JSON格式，可以被多种编程语言和平台解析和生成。
5. 可扩展性：JWT可以轻松地与其他技术（如OAuth）集成使用，适用于各种场景。

JWT由三部分组成：头部(header)、载荷(payload)和签名(signature)。

头部（header）： 头部通常由两部分组成，分别是令牌的类型（即"typ"字段，这里一般是"JWT"）和所采用的签名算法（即"alg"字段）。 例如：

```json
{
  "typ": "JWT",
  "alg": "HS256"
}
```

载荷（payload）： 载荷包含了一些与用户相关的信息，例如用户ID、角色、过期时间等。载荷可以自定义，并可添加任意数量的声明。 例如：

```json
{
  "sub": "1234567890",
  "name": "John Doe",
  "exp": 1619608137
}
```

1. Registered Claims（注册声明）： 这是一些预定义的声明，它们不是强制的，但是推荐在使用JWT时遵循它们。常见的注册声明包括：
	- "iss" (Issuer)：定义了令牌的发行者。
	- "sub" (Subject)：定义了令牌的主题，通常是用户的唯一标识符。
	- "aud" (Audience)：定义了令牌的受众，即接收令牌的对象。
	- "exp" (Expiration Time)：JWT令牌的过期日期。如果过期日期在过去，该令牌将无效。
	- "iat" (Issued At)：定义了令牌的发行时间。
	- "nbf"(Not Before)：声明定义了一个时间戳，表示令牌在此之前不可用或生效。
	- "jti" (JWT ID)：用于唯一标识JWT。每个JWT都应具有唯一的JTI值，可以帮助识别和防止重放攻击。
2. Public Claims（公共声明）： 公共声明不在JWT标准规范中进行预定义，但可以被不同的JWT实现和应用程序共同认可和理解，用于不同的应用程序之间的通信和交互。如：
	- name：表示用户的姓名。  
	- gender：表示用户的性别。  
	- avatar：表示用户的头像。  
	- role：表示用户的角色或权限。
1. Private Claims（私有声明）： 私有声明是应用程序特定的，通常只有特定的应用程序知道和使用这些声明，对其他未经授权的应用程序不可见和理解。

签名（signature）： 签名用于验证令牌的真实性和完整性，保证令牌没有被篡改。签名由使用私钥对头部和载荷进行加密生成。 例如：

```
HMACSHA256(
  base64UrlEncode(header) + "." +
  base64UrlEncode(payload),
  secret
)
```

将头部、载荷和签名用`.`连接起来，就形成了一个JWT令牌。 例如：`header.payload.signature`

## JWT的工作流程

1. 用户通过提供有效的凭证（例如用户名和密码）进行身份验证。
2. 服务器根据用户凭证生成一个JWT令牌，并将其返回给客户端。
3. 客户端将JWT令牌存储在本地，将其包含在后续的请求中的HTTP头部中。
4. 服务器接收到请求，解析JWT令牌，并根据其中的信息进行身份验证和授权。
5. 如果验证和授权成功，服务器会对请求进行处理，并返回响应给客户端。


## 代码示例

| 地址 | .Net版本 |
| -------- | -------- |
| [AspNetCore/Security/Authentication](https://github.com/happtim/aspnetcore-developer-roadmap/tree/main/AspNetCore/Security/Authentication)  | .Net6    | 

### Registering the Authentication Services

Microsoft.AspNetCore.Authentication.JwtBearer 包使在 ASP.NET Core 中实现 JWT 令牌验证更加容易。因此我们要安装它。该包是由微软AzureAD负责维护的。由于历史原因该包含两个命名空间的JWT库。
两个命名空间都在 Microsoft.AspNetCore.Authentication.JwtBearer 包内。

- System.IdentityModel.Tokens.Jwt
- Microsoft.IdentityModel.JsonWebTokens

`JwtSecurityTokenHandler`用于生成和验证JWT Token。它是微软推荐在.NET Framework项目中使用的JWT库。
`JsonWebTokenHandler` 这个库提供了对JWT的支持，包括生成和验证JWT Token的类和方法。它是微软推荐在ASP.NET Core项目中使用的JWT库。

本示例中两个命名空间的JWT库都使用，在生成JWT的代码中使用`JsonWebTokenHandler` 在验证Token代码中使用`JwtSecurityTokenHandler`

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  
.AddJwtBearer(options =>  
{    
	options.SaveToken = true; //表示将令牌保存在身份验证票据中。        
	options.TokenValidationParameters = new TokenValidationParameters        
	{  
		ValidateIssuer = true, //表示验证令牌的发行者。            
		ValidIssuer = jwtTokenConfig.Issuer, //用于指定有效的发行者                       
		ValidateAudience = true, //表示验证令牌的受众            
		ValidAudience = jwtTokenConfig.Audience, //用于指定有效的受众                        
		ValidateIssuerSigningKey = true,//表示验证令牌的签名密钥            
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)), //指定用于验证JWT签名的密钥                        
		RequireExpirationTime = false, //表示不要求令牌包含过期时间            
		ValidateLifetime = true, //表示验证令牌的有效期            
		ClockSkew = TimeSpan.Zero, //表示不允许任何时差        
	};  
});

builder.Services.AddScoped<JWTAuthService>();  
builder.Services.AddScoped<SignInManager>();  
builder.Services.AddSingleton<IUserService,UserService>();  
builder.Services.AddSingleton<IRefreshTokenService,RefreshTokenService>();
```

`AddJwtBearer` 方法调用的方法如下，内部是添加了“Bearer”的认证方案。
```csharp
public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, Action<JwtBearerOptions> configureOptions)  
{  
    return builder.AddJwtBearer("Bearer", configureOptions);  
}
```

### JWAuthService

```csharp
public class JWTAuthService
{
	private readonly JwtTokenConfig _jwtTokenConfig;
	public JWTAuthService(JwtTokenConfig jwtTokenConfig)
	{
		_jwtTokenConfig = jwtTokenConfig;
	}

	//创建JWT Token
	public string GenerateSecurityToken(Claim[] claims)
	{
		var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));
		var creds  = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
			Issuer = _jwtTokenConfig.Issuer,
			Audience = _jwtTokenConfig.Audience,
			SigningCredentials = creds
		};

		var tokenHandler = new JsonWebTokenHandler();
		var tokenString = tokenHandler.CreateToken(tokenDescriptor);
		
		return tokenString;
	}

	//创建 刷新Token，需要合理唯一 不被轻易被猜到。
	public string BuildRefreshToken()
	{
		var randomNumber = new byte[32];
		using (var randomNumberGenerator = RandomNumberGenerator.Create())
		{
			randomNumberGenerator.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}
	}
	
	// 从 JWT Token 中过去 User ( ClaimsPrincipal ) .
	public ClaimsPrincipal GetPrincipalFromToken(string token)
	{
		JwtSecurityTokenHandler tokenValidator = new JwtSecurityTokenHandler();
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var parameters = new TokenValidationParameters
		{
			ValidateAudience = false,
			ValidateIssuer = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = key,
			ValidateLifetime = false
		};

		try
		{
			var principal = tokenValidator.ValidateToken(token, parameters, out var securityToken);

			if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine($"Token validation failed");
				return null;
			}

			return principal;
		}
		catch (Exception e)
		{
			Console.WriteLine($"Token validation failed: {e.Message}");
			return null;
		}
	}
}

```

### Create JWT Token

我们从配置文件中获取密钥，然后创建SigningCredentials。

```csharp
var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));        
var creds  = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
```

创建一个新的`SecurityTokenDescriptor`。颁发者(Issuer)和受众(Audience)的值来自于配置文件。`AccessTokenExpiration`是根据当前时间添加到令牌过期声明中的。我们还需要传递`SigningCredentials`来创建令牌。`Subject` 使用用户的`Claims`。

```csharp
var tokenDescriptor = new SecurityTokenDescriptor
{
	Subject = new ClaimsIdentity(claims),
	Expires = DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
	Issuer = _jwtTokenConfig.Issuer,
	Audience = _jwtTokenConfig.Audience,
	SigningCredentials = creds
};

var tokenString = new JsonWebTokenHandler().CreateToken(tokenDescriptor);
```

### Refresh Tokens

JWT令牌使用exp声明来设置过期日期。没有过期日期，令牌将长时间有效。只要令牌的签名有效且令牌未过期，服务器将信任该令牌。如果黑客获取了令牌，他可以借此冒充真实用户。即使用户更改了密码，令牌仍然有效。唯一使其无效的方法是更改密钥，这将使所有令牌失效。

因此，我们需要设置较短的过期日期。但这会给真实用户带来问题，因为他们需要在令牌过期后重新登录以获取新的令牌。

这就是刷新令牌的用途。发行者会连同访问令牌一起发放刷新令牌。与访问令牌不同，发行者会安全地存储刷新令牌。刷新令牌也会过期，但其持续时间会比访问令牌长。

在访问令牌过期后，用户将向发行者提交刷新令牌。发行者验证刷新令牌并发放新的访问令牌以及新的刷新令牌。还会删除旧的刷新令牌，以防用户再次使用它。

所有这些都将在用户不知情的情况下发生。

### Create Refresh Token

方法BuildRefreshToken用于构建刷新令牌

创建刷新令牌有两个标准。令牌必须是相当独特且不容易被任何人猜出来的。

因此，我们使用一个32字节长的随机数，并将其转换为Base64编码，以便我们可以将其用作字符串。

```csharp
//创建 刷新Token，需要合理唯一 不被轻易被猜到。
public string BuildRefreshToken()
{
	var randomNumber = new byte[32];
	using (var randomNumberGenerator = RandomNumberGenerator.Create())
	{
		randomNumberGenerator.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}
}
```

GetPrincipalFromToken是从令牌中重新创建用户（ClaimsPrincipal）的方法。

我们使用JwtSecurityTokenHandler类的ValidateToken方法来实现这一点。ValidateToken方法要求我们传递要验证的令牌，SigningCredentials和TokenValidationParameters给它。

在刷新发放令牌时，我们使用此方法来验证访问令牌。

```csharp
// 从 JWT Token 中过去 User ( ClaimsPrincipal ) .
public ClaimsPrincipal GetPrincipalFromToken(string token)
{
	JwtSecurityTokenHandler tokenValidator = new JwtSecurityTokenHandler();
	var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));
	var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

	var parameters = new TokenValidationParameters
	{
		ValidateAudience = false,
		ValidateIssuer = false,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = key,
		ValidateLifetime = false
	};

	try
	{
		var principal = tokenValidator.ValidateToken(token, parameters, out var securityToken);

		if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
		{
			Console.WriteLine($"Token validation failed");
			return null;
		}

		return principal;
	}
	catch (Exception e)
	{
		Console.WriteLine($"Token validation failed: {e.Message}");
		return null;
	}
}
```

### Signing In

SignInManager会将用户登录。它将使用JWTAuthService创建访问和刷新令牌。

我们使用 SignIn 方法登录用户。

```csharp
public async Task<SignInResult> SignIn(string userName, string password)
{
	SignInResult result = new SignInResult();

	if (string.IsNullOrWhiteSpace(userName)) return result;
	if (string.IsNullOrWhiteSpace(password)) return result;

	//从数据库中验证用户名和密码
	var user = await _userService.Authenticate(userName,password);
	if (user != null)
	{
		//创建 claims
		var claims = BuildClaims(user);
		result.User = user;
		//使用_JwtAuthService创建 Access token & Refresh token
		result.AccessToken = _JwtAuthService.GenerateSecurityToken(claims);
		result.RefreshToken = _JwtAuthService.BuildRefreshToken();
		
		//保存 RefreshTokens to database
		_refreshTokenService.Add(new RefreshToken { UserId = user.Id, Token = result.RefreshToken, IssuedAt = DateTime.Now, ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration) });
		result.Success = true;
	};

	return result;
}
```


这段代码通过调用`_userService.Authenticate(userName, password)`方法从数据库中验证用户名和密码。如果验证成功（`user`不为空），接下来将构建`claims`并设置`result.User`为验证成功的用户。

然后，使用`_JwtAuthService.GenerateSecurityToken(claims)`方法创建访问令牌（Access token）并将其赋值给`result.AccessToken`。再调用`_JwtAuthService.BuildRefreshToken()`方法创建刷新令牌（Refresh token）并将其赋值给`result.RefreshToken`。

接下来，将刷新令牌和相关信息保存到数据库中，以便后续使用。`_refreshTokenService.Add()`方法将刷新令牌添加到数据库中。

RefreshToken方法验证当前的访问令牌和刷新令牌，并生成新的访问令牌和刷新令牌。

```csharp
public async Task<SignInResult> RefreshToken(string AccessToken, string RefreshToken)
{
	//使用当前Access Token恢复出 claimsPrincipal。
	ClaimsPrincipal claimsPrincipal = _JwtAuthService.GetPrincipalFromToken(AccessToken);
	SignInResult result = new SignInResult();

	if (claimsPrincipal == null) return result;

	//查找用户是否还存在。
	string id = claimsPrincipal.Claims.First(c => c.Type == "id").Value;
	var user = await _userService.FindAsync(Convert.ToInt32(id));

	if (user == null) return result;

	//查询数据库中的 RefreshToken 是否尚未过期
	var token = _refreshTokenService.GetAll()
			.Where(f => f.UserId == user.Id
					&& f.Token == RefreshToken
					&& f.ExpiresAt >= DateTime.Now)
			.FirstOrDefault();

	if (token == null) return result;

	var claims = BuildClaims(user);

	//创建新的AccessToken和RefreshToken
	result.User = user;
	result.AccessToken = _JwtAuthService.GenerateSecurityToken(claims);
	result.RefreshToken = _JwtAuthService.BuildRefreshToken();
	
	//删除旧的Token 并添加新的过去token。
	_refreshTokenService.Remove(token);
	_refreshTokenService.Add(new RefreshToken { UserId = user.Id, Token = result.RefreshToken, IssuedAt = DateTime.Now, ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration) });

	result.Success = true;

	return result;
}
```


该代码的功能是使用当前的 Access Token 恢复出 ClaimsPrincipal，然后根据 ClaimsPrincipal 查询用户信息和刷新令牌，最后创建新的 Access Token 和 Refresh Token，并更新数据库中的令牌信息。

具体步骤如下：
1. 使用 _JwtAuthService 的 GetPrincipalFromToken 方法将 Access Token 解析为 ClaimsPrincipal 对象。
2. 使用 _userService 的 FindAsync 方法根据 id 查找用户信息。
3. 根据用户 id、Refresh Token 和当前时间，从 _refreshTokenService 获取数据库中未过期的 Refresh Token。
4. 使用 BuildClaims 方法根据用户信息构建新的 Claim 列表。
5. 将用户信息、生成的 Access Token 和 Refresh Token 分别赋值给 result 对象的对应属性。
6. 使用 _refreshTokenService 的 Remove 方法删除旧的 Refresh Token。
7. 使用 _refreshTokenService 的 Add 方法将新的 Refresh Token 添加到数据库中。该 Refresh Token 包括用户 id、Token、颁发时间 IssuedAt 和过期时间 ExpiresAt。

最终，代码将会返回一个 SignInResult 对象，其中包含了用户信息、生成的 Access Token 和 Refresh Token。
