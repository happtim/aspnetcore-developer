<Query Kind="Statements">
  <NuGetReference Version="6.5.0">RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
</Query>

// Worker 执行过程将Worker标签Clone。然后执行两个worker。

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	//我们确保Rabbitmq服务宕机，确保消息不丢失，可是将参数durable设置true，
	channel.QueueDeclare(queue: "task_queue",
						 durable: true,
						 exclusive: false,
						 autoDelete: false,
						 arguments: null);

	//使用轮询分配消息，会将基数的消息都给一个worker。这样其实也没有均匀算量。
	//可以设置prefetchCount：1，将不会给Consumer消息，直到他发给我们ack消息。
	channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

	Console.WriteLine(" [*] Waiting for messages.");

	var consumer = new EventingBasicConsumer(channel);
	consumer.Received += (sender, ea) =>
	{
		var body = ea.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		Console.WriteLine(" [x] Received {0}", message);

		int dots = message.Split('.').Length - 1;
		Thread.Sleep(dots * 1000);

		Console.WriteLine(" [x] Done");

		// Note: it is possible to access the channel via
		//       ((EventingBasicConsumer)sender).Model here
		channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
	};
	
	//如果一个Worker正在执行，结果程序挂了，那么正在执行的任务就丢失了。
	//为了确保消息不会丢失，RabbitMq提供了消息确认机制。
	//当一个任务收到执行完，才发送要给ack消息让rabbitMq将该消息删除。
	channel.BasicConsume(queue: "task_queue",
						 autoAck: false,
						 consumer: consumer);

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}