using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers().RequireAuthorization();
app.MapReverseProxy();
app.MapFallbackToFile("index.html");

app.Run();
