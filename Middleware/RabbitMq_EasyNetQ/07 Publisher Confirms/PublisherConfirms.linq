<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
  <Namespace>EasyNetQ.Topology</Namespace>
</Query>


//默认的 AMQP 协议 发布不是事务性的，不能保证您的消息会实际到达Broker。
// RabbitMQ的实现，它非常慢，并且我们没有通过EasyNetQ API支持它。

const int MESSAGE_COUNT = 50_000;

//通过在连接字符串上设置 publisherConfirm=true 来启用发布者确认：
using (var bus = RabbitHutch.CreateBus("host=localhost;publisherConfirms=true;timeout=10"))
{
	var queue = bus.Advanced.QueueDeclare("", durable: false, exclusive: true, autoDelete: true);
	
	var timer = new Stopwatch();
	timer.Start();
	for (int i = 0; i < MESSAGE_COUNT; i++)
	{
		var message = new Message<TextMessage>(new TextMessage { Text = i.ToString() });

		//如果使用Publish。那么它就是在等待确认之后返回。如果没有在配置的timeout返回那么就会有异常。
		//如果考虑性能问题。使用异步的方式，这样在发布之后就会返回，出现异常之后以故障完成。
		bus.Advanced.PublishAsync(Exchange.Default, queue.Name, false, message)
			.ContinueWith(task =>
			{
				if (task.IsCompleted)
				{
					//Console.Out.WriteLine("{0} Completed", count);
				}
				if (task.IsFaulted)
				{
					Console.Out.WriteLine("\n\n");
					Console.Out.WriteLine(task.Exception);
					Console.Out.WriteLine("\n\n");
				}
			});
	}
	timer.Stop();
	Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages individually in {timer.ElapsedMilliseconds:N0} ms");
	
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


//[Queue("task_queue", ExchangeName = "")]
public class TextMessage
{
	public string Text { get; set; }
}
