using Duende.Bff.Yarp;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
        .MinimumLevel.Override("IdentityModel", LogEventLevel.Debug)
        .MinimumLevel.Override("Duende.Bff", LogEventLevel.Debug)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
            theme: AnsiConsoleTheme.Code));
    
    builder.Services.AddControllers();
    builder.Services.AddRazorPages();
    builder.Services
        .AddBff()
        .AddRemoteApis();
    
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
            options.Scope.Add("offline_access");
        });
    
    var app = builder.Build();

    app.UseSerilogRequestLogging();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }
    
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();
    
    app.UseRouting();
    app.UseAuthentication();
    app.UseBff();
    app.UseAuthorization();
    
    app.MapBffManagementEndpoints();
    app.MapRazorPages();

    app.MapControllers()
        .RequireAuthorization()
        .AsBffApiEndpoint();
    
    app.MapRemoteBffApiEndpoint("/remote", "https://localhost:6001")
        .RequireAccessToken(Duende.Bff.TokenType.User);

    app.MapFallbackToFile("index.html");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
