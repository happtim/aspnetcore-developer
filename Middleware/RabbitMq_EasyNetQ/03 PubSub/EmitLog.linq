<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	var exchange =  bus.Advanced.ExchangeDeclare("logs","fanout",durable:false);
	
	var message = new Message<TextMessage>(new TextMessage { Text = "info:HelloWorld;" });

	//mandatory
	//     This flag tells the server how to react if the message cannot be routed to a queue.
	//     If this flag is true, the server will return an unroutable message with a Return method.
	//     If this flag is false, the server silently drops the message.
	bus.Advanced.Publish(exchange, "", false, message);
	
	Console.WriteLine(" [x] Sent {0}", message.Body.Text);
	
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


//[Queue("task_queue", ExchangeName = "")]
public class TextMessage
{
	public string Text { get; set; }
}
