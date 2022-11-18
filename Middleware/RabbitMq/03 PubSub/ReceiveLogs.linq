<Query Kind="Statements">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
</Query>


// 每个日志接收器接受全部的日志消息，并不是其中的一部分。
// 每个日志接收器连接上时，接受最新的日志消息。

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
	
	// 上一个案例中，我们使用一个队列上连接了多个Consumer，轮询处理消息。
	// 但是在该案例中，就不能使用相同的Queue。
	
	//为了达到功能，我们需要创建一个随机的队列。并且当channel离线时，创建的队列自动删除。
	var queueName = channel.QueueDeclare().QueueName;
	
	//我们已经创建了一个exchange和queue。现在就需要把他们绑定在一起。
	channel.QueueBind(queue: queueName,
					  exchange: "logs",
					  routingKey: "");

	Console.WriteLine(" [*] Waiting for logs.");

	var consumer = new EventingBasicConsumer(channel);
	consumer.Received += (model, ea) =>
	{
		var body = ea.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		Console.WriteLine(" [x] {0}", message);
	};
	channel.BasicConsume(queue: queueName,
						 autoAck: true,
						 consumer: consumer);

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}