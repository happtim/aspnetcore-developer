<Query Kind="Statements">
  <NuGetReference Version="6.1.0">IdentityModel</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>IdentityModel.Client</Namespace>
</Query>

string tokenUrl = "https://localhost:5001/connect/token";

var client = new HttpClient();

//请求一个 token 使用 client_credentials 授权方式
var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
	Address = tokenUrl,
	ClientId = "client",
	ClientSecret = "secret",

 	Scope = "api1"
});

if (response.IsError) throw new Exception(response.Error);
var token = response.AccessToken.Dump("AccessToken");

//请求一个token 使用 authorization_code 授权方式
response = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
{
	Address = tokenUrl,

	ClientId = "client",
	ClientSecret = "secret",

	Code = "xyz",
	RedirectUri = "https://app.com/callback",

});
//if (response.IsError) throw new Exception(response.Error);
//token = response.AccessToken.Dump("AccessToken");

//请求一个token 使用 refresh_token 授权方式
response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
{
	Address = tokenUrl,

	ClientId = "web",
	ClientSecret = "secret",

	RefreshToken = "979DDF2BB0F74AEF869DF9AB48A92967771C4CA98B74E9DFE72429A2F9CF942C-1"
});
if (response.IsError) throw new Exception(response.Error);
token = response.AccessToken.Dump("AccessToken");