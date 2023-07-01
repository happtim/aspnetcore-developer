<Query Kind="Statements">
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


var builder = WebApplication.CreateBuilder();

builder.Configuration.AddInMemoryCollection(
	new[] {
		new KeyValuePair<string, string>("Jwt:Secret", "F-JaNdRfUserjd89#5*6Xn2r5usErw8x/A?D(G+KbPeShV"),
		new KeyValuePair<string, string>("Jwt:Issuer", "http://localhost:5000/"),
		new KeyValuePair<string, string>("Jwt:Audience", "http://localhost:5000/"),
		new KeyValuePair<string, string>("Jwt:AccessTokenExpiration", "5"),
		new KeyValuePair<string, string>("Jwt:RefreshTokenExpiration", "10"),
	});
string jwtIssuerOverride =  "http://localhost:5001/";
builder.Configuration.AddInMemoryCollection(
new[] {
	new KeyValuePair<string, string>("Jwt:Issuer",jwtIssuerOverride),
});

//builder.Configuration.AddJsonFile("custom.json");

var app = builder.Build();

var scopeFactory = app.Services.GetService<IServiceScopeFactory>();

using( var scope = scopeFactory.CreateScope() )
{
	var config  = scope.ServiceProvider.GetService<IConfiguration>();

	var configRoot = (IConfigurationRoot) config;
	
	foreach (var provider in configRoot.Providers.ToList())
	{
		provider.Dump(provider.GetType().Name);
	}
	
	config["Jwt:Issuer"].Dump("Jwt:Issuer value should be " + jwtIssuerOverride);
}

app.Run();