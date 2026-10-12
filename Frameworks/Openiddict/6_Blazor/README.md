# 具有后端服务的Blazor WASM clinet应用

与JavaScript单页面应用（SPAs）类似，您可以构建带有或不带有后端的Blazor WASM应用程序。不具备后端已经具有了我们在JavaScript入门讨论的所有安全缺点。

如果您构建的Blazor WASM应用程序不处理敏感数据，并且希望使用无后端的方法，请查看标准Microsoft模板，这些模板使用了这种风格。

## 配置后端

首先，在服务器项目中添加以下包引用：

```
dotnet package add Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet package add Yarp.ReverseProxy
```

接下来，我们将在后端添加OpenID Connect和OAuth支持。为此，我们将添加Microsoft OpenID Connect身份验证处理程序以进行与令牌服务的协议交互，以及用于管理生成的身份验证会话的Cookie身份验证处理程序。

```csharp
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "__Host-blazor";
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";
    
        // confidential client using code flow + PKCE
        options.ClientId = "bff";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.ResponseMode = "query";
    
        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
    
        // request scopes + refresh tokens
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("api1");
    });

// Create an authorization policy used by YARP when forwarding requests
// from the WASM application to the Dantooine.Api resource server.
builder.Services.AddAuthorization(options => options.AddPolicy("CookieAuthenticationPolicy", builder =>
{
    builder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
    builder.RequireAuthenticatedUser();
}));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builder => builder.AddRequestTransform(async context =>
    {
        // Attach the access token retrieved from the authentication cookie.
        //
        // Note: in a real world application, the expiration date of the access token
        // should be checked before sending a request to avoid getting a 401 response.
        // Once expired, a new access token could be retrieved using the OAuth 2.0
        // refresh token grant (which could be done transparently).
        var token = await context.HttpContext.GetTokenAsync(
            scheme: CookieAuthenticationDefaults.AuthenticationScheme,
            tokenName: "access_token");

        context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }));
```

最后一步是为认证、授权和BFF会话管理添加所需的中间件。在调用UseRouting后添加以下代码段：

```csharp
app.UseRouting();  
  
app.UseAuthentication();  
app.UseAuthorization();  
  
app.MapRazorPages();  
app.MapControllers().RequireAuthorization();  
app.MapReverseProxy();  
app.MapFallbackToFile("index.html");
```

最后你可以运行服务器项目。这将启动服务器，服务器将部署 Blazor 应用程序到你的浏览器。

## 修改前端代码

为Blazor应用程序添加安全性和身份验证需要几个步骤。

1. 将认证/授权相关的包添加到客户端项目文件中：
```
dotnet package add Microsoft.AspNetCore.Components.WebAssembly.Authentication
```

2. 在 _Imports.razor 中添加一个 using 语句，将上述包引入范围内：

```
@using Microsoft.AspNetCore.Components.Authorization
```

3. 为了将当前的身份验证状态传播到Blazor客户端的所有页面中，您需要在应用程序中添加一个特殊的组件称为CascadingAuthenticationState。在App.razor文件中使用这个组件来包裹Blazor路由器即可实现这一功能。

```xml
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)"/>
            <FocusOnNavigate RouteData="@routeData" Selector="h1"/>
        </Found>
        <NotFound>
            <PageTitle>Not found</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

4. 然后，我们将在布局页面中添加一些条件渲染，以便能够触发登录/注销，并在登录时显示当前用户名。这是通过在MainLayout.razor中使用AuthorizeView组件实现的。

```html
<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <div class="main">
        <div class="top-row px-4">
            <AuthorizeView>
                <Authorized>
                    <strong>Hello, @context.User.Identity.Name!</strong>
                    <a href="@context.User.FindFirst("bff:logout_url")?.Value">Log out</a>
                </Authorized>
                <NotAuthorized>
                    <a href="bff/login">Log in</a>
                </NotAuthorized>
            </AuthorizeView>
        </div>

        <div class="content px-4">
            @Body
        </div>
    </div>
</div>
```

然后添加 AuthenticationStateProvider

```csharp
using System.Net.Http.Json;
using System.Security.Claims;
using BlazorClient.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClient.Client.Services;

// Original source: https://github.com/berhir/BlazorWebAssemblyCookieAuth.
public class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private const string LogInPath = "api/Account/Login";
    private const string LogOutPath = "api/Account/Logout";

    private readonly NavigationManager _navigation;
    private readonly HttpClient _client;
    private readonly ILogger<HostAuthenticationStateProvider> _logger;

    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

    public HostAuthenticationStateProvider(NavigationManager navigation, HttpClient client, ILogger<HostAuthenticationStateProvider> logger)
    {
        _navigation = navigation;
        _client = client;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(await GetUser(useCache: true));
    }

    public void SignIn(string customReturnUrl = null)
    {
        var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
        var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? _navigation.Uri);
        var logInUrl = _navigation.ToAbsoluteUri($"{LogInPath}?returnUrl={encodedReturnUrl}");
        _navigation.NavigateTo(logInUrl.ToString(), true);
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
    {
        var now = DateTimeOffset.Now;
        if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
        {
            _logger.LogDebug("Taking user from cache");
            return _cachedUser;
        }

        _logger.LogDebug("Fetching user");
        _cachedUser = await FetchUser();
        _userLastCheck = now;

        return _cachedUser;
    }

    private async Task<ClaimsPrincipal> FetchUser()
    {
        UserInfo user = null;
        
        try
        {
            _logger.LogInformation(_client.BaseAddress.ToString());
            user = await _client.GetFromJsonAsync<UserInfo>("api/User");
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Fetching user failed.");
        }
        
        if (user == null || !user.IsAuthenticated)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var identity = new ClaimsIdentity(
            nameof(HostAuthenticationStateProvider),
            user.NameClaimType,
            user.RoleClaimType);

        if (user.Claims != null)
        {
            foreach (var claim in user.Claims)
            {
                identity.AddClaim(new Claim(claim.Type, claim.Value));
            }
        }

        return new ClaimsPrincipal(identity);
    }
}

```

…并在客户端的Program.cs注册它:

```csharp
builder.Services.AddAuthorizationCore(); builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();
```

在客户端的Program.cs文件中进行注册(覆盖标准的HTTP客户端配置；需要使用Microsoft.Extensions.Http包)：

```csharp
// HTTP client configuration
builder.Services.AddTransient<AntiforgeryHandler>();

builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AntiforgeryHandler>();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend"));
```

您可以通过将此代码添加到Index.razor来在主页上显示会话的内容。

```html
@page "/"

<PageTitle>Home</PageTitle>

<h1>Hello, Blazor BFF!</h1>

<AuthorizeView>
    <Authorized>
        <dl>
            @foreach (var claim in @context.User.Claims)
            {
                <dt>@claim.Type</dt>
                <dd>@claim.Value</dd>
            }
        </dl>
    </Authorized>
</AuthorizeView>
```

## 保护API

标准的 Blazor 模板中含有一个 API 端点（WeatherForecastController.cs）。尝试从 UI 调用天气页面。它可以在登录和匿名状态下均正常工作。我们希望修改代码，确保只有经过身份验证的用户能够调用 API。

在 ASP.NET Core 中的标准方法是，在控制器/操作或通过端点路由上添加授权要求，例如：

```csharp
app.MapControllers() 
	.RequireAuthorization();
```

当我尝试用匿名的方式调用API时，你会在浏览器控制台看到以下错误：

```
Access to fetch at 'https://localhost:5001/connect/authorize?client_id=bff&redirect_uri=https%3A%2F%2Flocalhost%3A5003%2Fsignin-oidc&response_type=code&scope=openid%20profile%20api1&code_challenge=tA5Whd52gIG06kiyw-BcTStm1STZJshkzLI--TG5mTQ&code_challenge_method=S256&nonce=638258663133431798.NmJiYjQ1OTAtMGYzMi00MjhlLWFkZWUtZDUwMDQ5YjkxOTkxNDQ5YmExY2QtMmNlZC00MDNlLWJlYjMtYWZjNjVkODk4Yjhm&state=CfDJ8PKG_H7sGGlEntm4WJzzr_NoAtQCZORyWaC8W0RL5A2jHZqY5RtFsfO3c9k2NPXct6bk_ADcoHQI41pmtpmXCV-lxYfK2tWbEgES7kH4Kqg5-7Z5dfeU6iiblFYNvSUwEh2XKaWpgVPjn4inFXz9NnBRkdsivMcY_LYV3u-zLr5zf3lH1YR0Bpyj7Yr4CHiK8LU-VYi35ZtfIAmSzCjwOQOfvpSNTOwnR9nFDCdh3KH6YCWnMWVGT9nyEBOBsezn50gT3GAM5Mw0J0f4M70jn0vYP3nAh4Oejo4-TaMnWj32Ak5QcuHZb-RC9mgV9-pMJHQPfpvvGpCDzrHYDHoGy1J5QjU57bPMMGB3-8-GEevlTYpSaOjnTsZOQr9TG8-WXrA9L4-f3uUA6Snpn01Tr4g&x-client-SKU=ID_NETSTANDARD2_0&x-client-ver=6.10.0.0' (redirected from 'https://localhost:5003/WeatherForecast') from origin 'https://localhost:5003' has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present on the requested resource. If an opaque response serves your needs, set the request's mode to 'no-cors' to fetch the resource with CORS disabled.
```

这是因为ASP.NET Core身份验证管道正在触发重定向到OpenID Connect提供程序进行身份验证。在这种情况下，我们实际上需要的是一个API友好的状态码-在这种情况下是401。

添加AuthorizedHandler，用来判断当前用户的认证状态。

```csharp
// Original source: https://github.com/berhir/BlazorWebAssemblyCookieAuth.
public class AuthorizedHandler : DelegatingHandler
{
    private readonly HostAuthenticationStateProvider _authenticationStateProvider;

    public AuthorizedHandler(HostAuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        HttpResponseMessage responseMessage;
        if (!authState.User.Identity.IsAuthenticated)
        {
            // if user is not authenticated, immediately set response status to 401 Unauthorized
            responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
        else
        {
            responseMessage = await base.SendAsync(request, cancellationToken);
        }

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            // if server returned 401 Unauthorized, redirect to login page
            //_authenticationStateProvider.SignIn();
        }

        return responseMessage;
    }
}

```

请求后端的http请求中，添加AuthorizedHandler处理。

```csharp
builder.Services.AddHttpClient("authorizedClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
}).AddHttpMessageHandler<AuthorizedHandler>();
```

在进行这个更改之后，您应该能够看到一个更好的错误信息。

```
Response status code does not indicate success: 401 (Unauthorized).
```

客户端代码可以响应这个状态码，例如触发登录重定向。

当您现在登录并调用API时，您可以在服务器端设置断点并检查API控制器是否可以通过.User属性访问验证用户的声明。