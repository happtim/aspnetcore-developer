<Query Kind="Statements">
  <NuGetReference Version="6.5.0">RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
</Query>


//Work Queues：主要思想就是避免密集计算型任务，必需等待他完成。
//相反，我们可以将一个耗时的任务封装成一个消息把他发给消息队列。可以有很多的Workers程序从队列中获取消息并执行任务。
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection()) 
using (var channel = connection.CreateModel())
{
	channel.QueueDeclare(queue: "task_queue",
							   durable: true,
							   exclusive: false,
							   autoDelete: false,
							   arguments: null);

	//通过.来将worker线程Sleep。代表这个任务执行的耗时程度。
	foreach(var message in new string[] {"First.","Second..","Third...","Fourth....","Fifth....."})
	{
		var body = Encoding.UTF8.GetBytes(message);
	
		//不光将队列设置成durable：true，也需要设置消息设置Persistent：true。才能保存消息。
		var properties = channel.CreateBasicProperties();
		properties.Persistent = true;

		//默认情况RabbitMq会将消息平均分配给消费者。（round-robin）轮询。
		channel.BasicPublish(exchange: "",
							 routingKey: "task_queue",
							 basicProperties: properties,
							 body: body);
							 
		Console.WriteLine(" [x] Sent {0}", message);
	}

}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
