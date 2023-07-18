<Query Kind="Statements">
  <NuGetReference Version="6.1.0">IdentityModel</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>IdentityModel.Client</Namespace>
</Query>

string tokenUrl = "https://localhost:5001/connect/token";
string introspectUrl = "https://localhost:5001/connect/introspect";

var client = new HttpClient();

var accessToken = "";
var response = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
{
	Address = introspectUrl,
	ClientId = "web",
	ClientSecret = "secret",

	Token = accessToken,
});

if (response.IsError) throw new Exception(response.Error);
response.Dump();
