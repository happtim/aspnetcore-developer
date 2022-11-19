<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>

using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	Console.WriteLine(" [x] Requesting fib(43)");
	var response = bus.Rpc.Request<int,int>(43);
	Console.WriteLine(" [.] Got '{0}'", response);
	
}
