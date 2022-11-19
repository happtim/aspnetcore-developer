<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	var exchange = bus.Advanced.ExchangeDeclare("topic_logs", "topic", durable: false);

	var args = new string[] { "quick.orange.fox", "quick.orange.rabbit", "lazy.orange.elephant", "lazy.pink.rabbit" };


	foreach (var routingKey in args)
	{
		var helloWorld = "HelloWorld";

		var message = new Message<TextMessage>(new TextMessage { Text = helloWorld });

		bus.Advanced.Publish(exchange, routingKey, false, message);

		Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message.Body.Text);
	}
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


//[Queue("task_queue", ExchangeName = "")]
public class TextMessage
{
	public string Text { get; set; }
}
