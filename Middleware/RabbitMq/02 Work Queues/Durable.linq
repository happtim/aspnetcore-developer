<Query Kind="Statements">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
</Query>

//重新声明hello消息队列为持久化的。 
//此时会报错。rabbitmq不支持对已有的消息队列重定义
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	//这里我们同样声明了一个队列。这里可能consumer 优先在 publisher 启动。我们要确保消息队列的存在，才能获取消息。
	channel.QueueDeclare(queue: "hello",
						 durable: true,
						 exclusive: false,
						 autoDelete: false,
						 arguments: null);

	var consumer = new EventingBasicConsumer(channel);
	consumer.Received += (model, ea) =>
	{
		var body = ea.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		Console.WriteLine(" [x] Received {0}", message);
	};
	channel.BasicConsume(queue: "hello",
						 autoAck: true,
						 consumer: consumer);

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}