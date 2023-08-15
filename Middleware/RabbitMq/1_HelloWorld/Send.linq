<Query Kind="Statements">
  <NuGetReference Version="6.5.0">RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
</Query>


var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection()) //Connection是一个socket的抽象，用于协议版本，认证功能。
using (var channel = connection.CreateModel()) 
{
	// 声明一个队列， 如果没有队列则创建一个。
	channel.QueueDeclare(queue: "hello",
							   durable: false,
							   exclusive: false,
							   autoDelete: false,
							   arguments: null);

	string message = "Hello World!";
	var body = Encoding.UTF8.GetBytes(message);

	//这里exchange 为空，我们还未引入exchange的概念。
	channel.BasicPublish(exchange: "",
						 routingKey: "hello",
						 basicProperties: null,
						 body: body);
	Console.WriteLine(" [x] Sent {0}", message);
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();