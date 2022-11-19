<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{	
	var queue = bus.Advanced.QueueDeclare("task_queue",durable:true,exclusive:false,autoDelete:false);

	bus.Advanced.Consume<TextMessage>(queue, (message, info) => 
		{
			Console.WriteLine(" [x] Received {0}", message.Body.Text);
			int dots = message.Body.Text.Split('.').Length - 1;
			Thread.Sleep(dots * 1000);

			Console.WriteLine(" [x] Done");
		},
		conf =>conf.WithPrefetchCount(1));
	
	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}

public class TextMessage
{
	public string Text { get; set; }
}
