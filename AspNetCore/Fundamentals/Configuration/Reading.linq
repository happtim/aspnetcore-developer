<Query Kind="Statements">
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
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
	
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Jwt));
	
var app = builder.Build();

var scopeFactory = app.Services.GetService<IServiceScopeFactory>();

using (var scope = scopeFactory.CreateScope())
{
	var config = scope.ServiceProvider.GetService<IConfiguration>();
	
	//Index
	config["Jwt:Issuer"].Dump("Jwt:Issuer");
	config["JWT:Issuer"].Dump("JWT:Issuer");
	config["JWT:Issuerr"].Dump("JWT:Issuerr");
	
	//GetValue
	config.GetValue<int>("Jwt:AccessTokenExpiration").Dump("Jwt:AccessTokenExpiration");
	
	//GetSection
	var jwtConfig =  config.GetSection("Jwt");
	jwtConfig["Issuer"].Dump("Issuer");
	
	//GetChildren
	var children = jwtConfig.GetChildren();
	foreach (var child in children)
	{
		child.Value.Dump(child.Path);
	}
	
	//Bind
	var jwtOptions = new JwtOptions();
	jwtConfig.Bind(jwtOptions);
	
	jwtOptions.Dump(nameof(jwtOptions));
	
	//Options
	var options =  scope.ServiceProvider.GetService<IOptions<JwtOptions>>();
	options.Value.Dump("jwtOptions");
	
}

app.Run();

class JwtOptions 
{
	public const string Jwt = "Jwt";
	public string Secret {get;set;} = string.Empty;
	public string Issuer {get;set;} = string.Empty;
}