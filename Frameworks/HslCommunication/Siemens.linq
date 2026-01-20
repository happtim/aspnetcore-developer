<Query Kind="Statements">
  <NuGetReference Version="12.0.3">HslCommunication</NuGetReference>
  <Namespace>HslCommunication.Profinet.Melsec</Namespace>
  <Namespace>HslCommunication.Profinet.Siemens</Namespace>
</Query>



var client = new SiemensS7Net( SiemensPLCS.S1200);
client.IpAddress = "127.0.0.1";


await client.WriteAsync("DB300.10", "12345678",UTF8Encoding.UTF8);

var result = await client.ReadStringAsync("DB100.10",0);


result.Dump();