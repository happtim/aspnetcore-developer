# 具有后端服务的Blazor WASM clinet应用

与JavaScript单页面应用（SPAs）类似，您可以构建带有或不带有后端的Blazor WASM应用程序。不具备后端已经具有了我们在JavaScript入门讨论的所有安全缺点。

如果您构建的Blazor WASM应用程序不处理敏感数据，并且希望使用无后端的方法，请查看标准Microsoft模板，这些模板使用了这种风格。

在本快速入门中，我们将重点介绍如何使用我们的Duende.BFF安全框架构建Blazor WASM应用程序。

### Setting up the project

.NET 6 CLI包含具有后端的Blazor WASM模板。在您要开始工作的目录中创建目录，然后运行以下命令：

```
dotnet new blazorwasm --hosted

```

Blazor WebAssembly 应用程序有两种托管模式:

1. 独立托管(Standalone) - 整个应用程序包括客户端和服务器端都打包在一个项目中,并作为静态文件一起部署。
2. 服务器托管(Hosted) - 客户端项目依赖一个服务器端项目。客户端包含浏览器运行的 WebAssembly,服务器端是一个 ASP.NET Core 应用,用于托管和服务端API。

使用 --hosted 参数会创建一个服务器托管的解决方案,包含两个项目:一个客户端 Blazor WebAssembly 应用和一个 ASP.NET Core 服务器应用。

服务器应用负责托管客户端应用并提供 Web API。客户端通过 HttpClient 调用服务器端 API。这种模式下,服务器端可以处理诸如认证、授权、数据访问等服务端关注点。

### Configuring the backend

首先，在服务器项目中添加以下包引用：

```
<PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="6.0.0" />
<PackageReference Include="Duende.BFF" Version="1.1.0" />
```


接下来，我们将在后端添加OpenID Connect和OAuth支持。为此，我们将添加Microsoft OpenID Connect身份验证处理程序以进行与令牌服务的协议交互，以及用于管理生成的身份验证会话的Cookie身份验证处理程序。有关更多背景信息，请参见此处。

BFF服务提供了从前端调用身份验证框架的逻辑（稍后会有更多介绍）。

```csharp
builder.Services.AddBff();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "__Host-blazor";
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://demo.duendesoftware.com";

        options.ClientId = "interactive.confidential";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("api");
        options.Scope.Add("offline_access");

        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
    });

```

最后一步是为认证、授权和BFF会话管理添加所需的中间件。在调用UseRouting后添加以下代码段：

```csharp
app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapBffManagementEndpoints();

```

最后你可以运行服务器项目。这将启动服务器，服务器将部署 Blazor 应用程序到你的浏览器。

尝试手动调用 /bff/login 上的 BFF 登录端点-这应该将你带到演示 Identity Server。登录后（例如，使用 bob/bob），浏览器将返回到 Blazor 应用程序。

换句话说，基本的身份验证已经在运行中。现在我们需要让前端知道这一点。

### Modifying the frontend (part 1)

为Blazor应用程序添加安全性和身份验证需要几个步骤。

1. 将认证/授权相关的包添加到客户端项目文件中：

```
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="6.0.0" />
```

2. 在 _Imports.razor 中添加一个 using 语句，将上述包引入范围内：

```
@using Microsoft.AspNetCore.Components.Authorization
```

3. 为了将当前的身份验证状态传播到Blazor客户端的所有页面中，您需要在应用程序中添加一个特殊的组件称为CascadingAuthenticationState。在App.razor文件中使用这个组件来包裹Blazor路由器即可实现这一功能。

```html
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

当您现在运行Blazor应用程序时，您将在浏览器控制台中看到以下错误:

```
crit: Microsoft.AspNetCore.Components.WebAssembly.Rendering.WebAssemblyRenderer[100]
      Unhandled exception rendering component: Cannot provide a value for property 'AuthenticationStateProvider' on type 'Microsoft.AspNetCore.Components.Authorization.CascadingAuthenticationState'. There is no registered service of type 'Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider'.

```

CascadingAuthenticationState是对任意身份验证系统的抽象。它在内部依赖于一个名为AuthenticationStateProvider的服务，以返回有关当前身份验证状态和当前登录用户的所需信息。

需要实现此组件，接下来我们将这样做。

### Modifying the frontend (part 2)

BFF库有一个服务器端组件，可以查询当前的身份验证会话和状态（请参见此处）。现在我们将添加一个Blazor AuthenticationStateProvider，它将在内部使用该终点。

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor6.Client.BFF;

public class BffAuthenticationStateProvider 
    : AuthenticationStateProvider
{
    private static readonly TimeSpan UserCacheRefreshInterval 
        = TimeSpan.FromSeconds(60);

    private readonly HttpClient _client;
    private readonly ILogger<BffAuthenticationStateProvider> _logger;

    private DateTimeOffset _userLastCheck 
        = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedUser 
        = new ClaimsPrincipal(new ClaimsIdentity());

    public BffAuthenticationStateProvider(
        HttpClient client,
        ILogger<BffAuthenticationStateProvider> logger)
    {
        _client = client;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(await GetUser());
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = true)
    {
        var now = DateTimeOffset.Now;
        if (useCache && now < _userLastCheck + UserCacheRefreshInterval)
        {
            _logger.LogDebug("Taking user from cache");
            return _cachedUser;
        }

        _logger.LogDebug("Fetching user");
        _cachedUser = await FetchUser();
        _userLastCheck = now;

        return _cachedUser;
    }

    record ClaimRecord(string Type, object Value);

    private async Task<ClaimsPrincipal> FetchUser()
    {
        try
        {
            _logger.LogInformation("Fetching user information.");
            var response = await _client.GetAsync("bff/user?slide=false");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var claims = await response.Content.ReadFromJsonAsync<List<ClaimRecord>>();

                var identity = new ClaimsIdentity(
                    nameof(BffAuthenticationStateProvider),
                    "name",
                    "role");

                foreach (var claim in claims)
                {
                    identity.AddClaim(new Claim(claim.Type, claim.Value.ToString()));
                }

                return new ClaimsPrincipal(identity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fetching user failed.");
        }

        return new ClaimsPrincipal(new ClaimsIdentity());
    }
}

```

...并在客户端的Program.cs注册它:

```csharp
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();
```

如果您现在再次运行服务器应用程序，您将看到一个不同的错误：

```
fail: Duende.Bff.Endpoints.BffMiddleware[1]
      Anti-forgery validation failed. local path: '/bff/user'
```

这是由于在BFF主机的管理端点上自动应用的防伪保护导致的。为了正确保护调用，您需要为调用添加一个静态的X-CSRF头。有关更多背景信息，请参见这里。

可以通过一个委托处理程序来轻松实现这一目标，该处理程序可插入到Blazor前端使用的默认HTTP客户端中。让我们首先添加这个处理程序：

```csharp
public class AntiforgeryHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-CSRF", "1");
        return base.SendAsync(request, cancellationToken);
    }
}

```

...并在客户端的Program.cs文件中进行注册(覆盖标准的HTTP客户端配置；需要使用Microsoft.Extensions.Http包)：

```csharp
// HTTP client configuration
builder.Services.AddTransient<AntiforgeryHandler>();

builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AntiforgeryHandler>();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend"));

```

如果您再次启动应用程序，登录/登出逻辑现在应该可以正常运行。此外，您可以通过将此代码添加到Index.razor来在主页上显示会话的内容。

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


### Securing the local API

标准的 Blazor 模板中含有一个 API 端点（WeatherForecastController.cs）。尝试从 UI 调用天气页面。它可以在登录和匿名状态下均正常工作。我们希望修改代码，确保只有经过身份验证的用户能够调用 API。

在 ASP.NET Core 中的标准方法是，在控制器/操作或通过端点路由上添加授权要求，例如：

```csharp
app.MapControllers()
        .RequireAuthorization();

```

当你现在尝试匿名调用API时，你会在浏览器控制台中看到以下错误：

```
Access to fetch at 'https://demo.duendesoftware.com/connect/authorize?client_id=...[shortened]... (redirected from 'https://localhost:5002/WeatherForecast') from origin 'https://localhost:5002' has been blocked by CORS policy: Response to preflight request doesn't pass access control check: No 'Access-Control-Allow-Origin' header is present on the requested resource. If an opaque response serves your needs, set the request's mode to 'no-cors' to fetch the resource with CORS disabled.
```

这是因为ASP.NET Core身份验证管道正在触发重定向到OpenID Connect提供程序进行身份验证。在这种情况下，我们实际上需要的是一个API友好的状态码-在这种情况下是401。

这是BFF中间件的一个特性，但是您需要将端点标记为BFF API端点才能生效：

```
app.MapControllers()
        .RequireAuthorization()
        .AsBffApiEndpoint();
```

在进行这个更改之后，您应该能够看到一个更好的错误信息。

```
Response status code does not indicate success: 401 (Unauthorized).
```

客户端代码可以响应这个状态码，例如触发登录重定向。

当您现在登录并调用API时，您可以在服务器端设置断点并检查API控制器是否可以通过.User属性访问验证用户的声明。

