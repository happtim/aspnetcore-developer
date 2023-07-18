<Query Kind="Statements">
  <NuGetReference Version="6.1.0">IdentityModel</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>IdentityModel.Client</Namespace>
</Query>

string url = "https://localhost:5001";

var client = new HttpClient();

var disco = await client.GetDiscoveryDocumentAsync(url);
if (disco.IsError) throw new Exception(disco.Error);

//请求一个 token 使用 client_credentials 授权方式
var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
	Address = disco.TokenEndpoint,
	ClientId = "client",
	ClientSecret = "secret",

	Scope = "api1"
});

if (response.IsError) throw new Exception(response.Error);
var accessToken = response.AccessToken;

var response2 = await client.GetUserInfoAsync(new UserInfoRequest
{
	Address = disco.UserInfoEndpoint,
	Token = accessToken
});

if (response2.IsError) throw new Exception(response2.Error);
 response2.Dump();