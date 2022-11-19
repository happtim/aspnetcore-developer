<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	var exchange = bus.Advanced.ExchangeDeclare("direct_logs", "direct", durable: false);
	
	var args = new string[] {"info", "warning", "error"};

	foreach (var severity in args)
	{
		var helloWorld = "HelloWorld";
		
		var message = new Message<TextMessage>(new TextMessage { Text = helloWorld });

		bus.Advanced.Publish(exchange, severity, false, message);

		Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message.Body.Text);
	}
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


//[Queue("task_queue", ExchangeName = "")]
public class TextMessage
{
	public string Text { get; set; }
}
