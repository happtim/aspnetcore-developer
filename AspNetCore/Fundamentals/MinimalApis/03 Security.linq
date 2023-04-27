<Query Kind="Statements">
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Authentication</Namespace>
  <Namespace>Microsoft.AspNetCore.Authentication.Cookies</Namespace>
  <Namespace>Microsoft.AspNetCore.Authorization</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Security.Claims</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


var builder = WebApplication.CreateBuilder();
// Requires Microsoft.AspNetCore.Authentication.JwtBearer
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => {
		options.LoginPath = "/login";
		options.AccessDeniedPath = "/denied";
	});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", [Authorize] () => "Protected Resource");
app.MapGet("/login", async (HttpContext  context) =>
{
	var claims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, "Alice"),
		new Claim(ClaimTypes.Role, "Admin")
	};
	var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
	var principal = new ClaimsPrincipal(identity);
	await context.SignInAsync(principal);
	return new { message = "Logged in" };
});
app.MapGet("/denied", () => "Access Denied");

Process.Start(new ProcessStartInfo
{
	FileName = "http://localhost:5000/swagger/index.html",
	UseShellExecute = true,
});

app.Run();