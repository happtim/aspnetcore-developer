<Query Kind="Statements">
  <NuGetReference Version="12.0.3">HslCommunication</NuGetReference>
  <Namespace>HslCommunication.Profinet.Melsec</Namespace>
</Query>



var client = new MelsecMcNet("10.179.0.162", 6001);

var result = client.ReadBool("X0",1);

result.Dump();