<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
  <Namespace>EasyNetQ.Topology</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	
	bus.Advanced.QueueDeclare("task_queue",durable:true,exclusive:false,autoDelete:false);
	//通过.来将worker线程Sleep。代表这个任务执行的耗时程度。
	foreach (var text in new string[] { "First.", "Second..", "Third...", "Fourth....", "Fifth....." })
	{
		var message = new Message<TextMessage>(new TextMessage { Text = text });
		message.Properties.DeliveryMode = 2;
		bus.Advanced.Publish(Exchange.Default, "task_queue", false, message);
		Console.WriteLine(" [x] Sent {0}", text);
	}
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


//[Queue("task_queue", ExchangeName = "")]
public class TextMessage
{
	public string Text { get; set; }
}
