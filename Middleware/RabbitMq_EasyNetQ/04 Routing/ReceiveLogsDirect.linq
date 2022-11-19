<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>



//clone 本文件的参数配置
//var args = new string[] {"error"}; 
var args = new string[] { "info", "warning", "error" };

using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	var exchange = bus.Advanced.ExchangeDeclare("direct_logs", "direct", durable: false);
	var queue = bus.Advanced.QueueDeclare("", durable: false, exclusive: true, autoDelete: true);
	
	foreach(var severity in args)
	{
		bus.Advanced.Bind(exchange, queue, severity);
	}

	bus.Advanced.Consume<TextMessage>(queue, (message, info) =>
		{
			Console.WriteLine(" [x] Received '{0}':'{1}'", info.RoutingKey, message.Body.Text);
			
		},conf => conf .WithAutoAck());

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}

public class TextMessage
{
	public string Text { get; set; }
}
