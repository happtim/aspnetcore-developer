<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>

using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	bus.Rpc.Respond<int, int>(i => 
	{
		Console.WriteLine(" [.] fib({0}) {1}", i,DateTime.Now);
		return fib(i);
	});

	Console.WriteLine(" [x] Awaiting RPC requests");

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();

}


static int fib(int n)
{
	if (n == 0 || n == 1)
	{
		return n;
	}

	return fib(n - 1) + fib(n - 2);
}