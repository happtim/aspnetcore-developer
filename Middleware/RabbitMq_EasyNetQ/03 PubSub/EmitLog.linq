<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	var exchange =  bus.Advanced.ExchangeDeclare("logs","fanout",durable:false);
	
	var message = new Message<TextMessage>(new TextMessage { Text = "info:HelloWorld;" });
	
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
