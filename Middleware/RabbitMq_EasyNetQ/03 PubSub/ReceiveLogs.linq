<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	var exchange =bus.Advanced.ExchangeDeclare("logs","fanout",durable:false);
	var queue = bus.Advanced.QueueDeclare("",durable:false,exclusive:true,autoDelete:true);
	bus.Advanced.Bind(exchange,queue,"");
	
	bus.Advanced.Consume<TextMessage>(queue, (message, info) =>
		{
			Console.WriteLine(" [x] {0}", message.Body.Text);
		},conf => conf.WithAutoAck());

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}

public class TextMessage
{
	public string Text { get; set; }
}
