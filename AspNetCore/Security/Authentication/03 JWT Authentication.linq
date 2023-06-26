<Query Kind="Statements">
  <NuGetReference Version="6.0.16">Microsoft.AspNetCore.Authentication.JwtBearer</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Authentication.JwtBearer</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.Configuration.Memory</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.IdentityModel.Tokens</Namespace>
  <Namespace>System.IdentityModel.Tokens.Jwt</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>Microsoft.AspNetCore.Authorization</Namespace>
  <Namespace>Microsoft.OpenApi.Models</Namespace>
  <Namespace>Swashbuckle.AspNetCore.SwaggerUI</Namespace>
  <Namespace>Microsoft.IdentityModel.JsonWebTokens</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load ".\UserService"

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	EnvironmentName = Environments.Development
});

builder.Configuration.AddInMemoryCollection(
	new[] {
		new KeyValuePair<string, string>("Jwt:Secret", "F-JaNdRfUserjd89#5*6Xn2r5usErw8x/A?D(G+KbPeShV"),
		new KeyValuePair<string, string>("Jwt:Issuer", "http://localhost:5000/"),
		new KeyValuePair<string, string>("Jwt:Audience", "http://localhost:5000/"),
		new KeyValuePair<string, string>("Jwt:AccessTokenExpiration", "5"),
		new KeyValuePair<string, string>("Jwt:RefreshTokenExpiration", "10"),
	});

var jwtTokenConfig =builder.Configuration.GetSection("Jwt").Get<JwtTokenConfig>();
builder.Services.AddSingleton(jwtTokenConfig);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{  
		options.SaveToken = true; //表示将令牌保存在身份验证票据中。
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true, //表示验证令牌的发行者。
			ValidIssuer = jwtTokenConfig.Issuer, //用于指定有效的发行者
			
			ValidateAudience = true, //表示验证令牌的受众
			ValidAudience = jwtTokenConfig.Audience, //用于指定有效的受众
			
			ValidateIssuerSigningKey = true,//表示验证令牌的签名密钥
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)), //指定用于验证JWT签名的密钥
			
			RequireExpirationTime = false, //表示不要求令牌包含过期时间
			ValidateLifetime = true, //表示验证令牌的有效期
			ClockSkew = TimeSpan.Zero, //表示不允许任何时差
		};
	});
	
builder.Services.AddScoped<JWTAuthService>();
builder.Services.AddScoped<SignInManager>();
builder.Services.AddSingleton<IUserService,UserService>();
builder.Services.AddSingleton<IRefreshTokenService,RefreshTokenService>();

// Authorization middleware
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.
                          Example: 'Bearer 12345abcdef'",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer"
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] { }
		}
	});
});


// Configure pipeline
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
app.MapPost("/login", async (LoginRequest login, SignInManager _signInManager) =>
{
   	var result= await _signInManager.SignIn(login.UserName, login.Password);

	if (!result.Success) 
		return Results.Unauthorized();

  	return Results.Ok(new LoginResult()
    {
        UserName = result.User.Id.ToString(),
        AccessToken = result.AccessToken,
        RefreshToken = result.RefreshToken
    });
});

app.MapPost("/refreshtoken", async (RefreshTokenRequest request,SignInManager _signInManager) =>
{
	var result = await _signInManager.RefreshToken(request.AccessToken, request.RefreshToken);

	if (!result.Success) 
		return Results.Unauthorized();

  	return Results.Ok(new LoginResult()
    {
        UserName = result.User.Id.ToString(),
		  AccessToken = result.AccessToken,
		  RefreshToken = result.RefreshToken
	  });
});

Process.Start(new ProcessStartInfo
{
	FileName = "http://localhost:5000/swagger/index.html",
	UseShellExecute = true,
});

app.Run();

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

		var tokenString = new JsonWebTokenHandler().CreateToken(tokenDescriptor);
		
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

public class JwtTokenConfig
{
	public string Secret { get; set; }
	public string Issuer { get; set; }
	public string Audience { get; set; }
	public int AccessTokenExpiration { get; set; }
	public int RefreshTokenExpiration { get; set; }
}

public class SignInManager
{
	private readonly JWTAuthService _JwtAuthService;
	private readonly JwtTokenConfig _jwtTokenConfig;
	private readonly IUserService _userService;
	private readonly IRefreshTokenService _refreshTokenService;

	public SignInManager(
						 JWTAuthService JWTAuthService,
						 JwtTokenConfig jwtTokenConfig,
						 IUserService userService,
						 IRefreshTokenService refreshTokenService
						 )
	{
		_userService = userService;
		_JwtAuthService = JWTAuthService;
		_jwtTokenConfig = jwtTokenConfig;
		_refreshTokenService = refreshTokenService;
	}

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

	//用于验证当前 Access Token & Refresh Token 如过正确重新 Access Token & Refresh Token
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

	private Claim[] BuildClaims(User user)
	{
		//User is Valid
		var claims = new[]
		{
				new Claim("id",user.Id.ToString()),
				new Claim(ClaimTypes.Name,user.Username),
 				new Claim("UserDefined", "whatever"),
                //Add Custom Claims here
            };

		return claims;
	}
}

public class SignInResult
{
	public bool Success { get; set; }
	public User User { get; set; }
	public string AccessToken { get; set; }
	public string RefreshToken { get; set; }

	public SignInResult()
	{
		Success = false;
	}
}

public class RefreshTokenService : IRefreshTokenService
{
	private readonly List<RefreshToken> tokens = new List<RefreshToken>();
	public void Add(RefreshToken token)
	{
		tokens.Add(token);
	}

	public IList<RefreshToken> GetAll()
	{
		return tokens;
	}

	public void Remove(RefreshToken token)
	{
		tokens.Remove(token);
	}
}

public interface IRefreshTokenService
{
	public void Add(RefreshToken token);
	public void Remove(RefreshToken token);
	public IList<RefreshToken> GetAll();
}

public class RefreshToken
{
	public string Token { get; set; }
	public int UserId { get; set; }
	public DateTime IssuedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
}

public class LoginResult
{
	public string UserName { get; set; }
	public string AccessToken { get; set; }
	public string RefreshToken { get; set; }
}

public class RefreshTokenRequest
{
	public string AccessToken { get; set; }
	public string RefreshToken { get; set; }
}