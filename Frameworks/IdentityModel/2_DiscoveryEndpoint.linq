<Query Kind="Statements">
  <NuGetReference Version="6.1.0">IdentityModel</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>IdentityModel.Client</Namespace>
</Query>

string url = "https://localhost:5001";

var client = new HttpClient();

var disco = await client.GetDiscoveryDocumentAsync(url);
if (disco.IsError) throw new Exception(disco.Error);

var tokenEndpoint = disco.TokenEndpoint.Dump("tokenEndpoint");
var keys = disco.KeySet.Keys.Dump("keys");

