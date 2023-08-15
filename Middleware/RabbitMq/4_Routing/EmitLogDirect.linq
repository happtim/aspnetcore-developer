<Query Kind="Statements">
  <NuGetReference Version="6.5.0">RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
</Query>

// 该案例中我们使得订阅消息子集成为可能， 可以将错误级别的消息定向文件（节省磁盘）。控制台上还是所有显示日志消息。
//
var args = new string[] {"info", "warning", "error"};
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	channel.ExchangeDeclare(exchange: "direct_logs",type: "direct");
	
	foreach(var severity in args)
	{

		var message =  $"Hello World!";
		var body = Encoding.UTF8.GetBytes(message);

		// 发出消息时候我们将指定routingkey的日志级别。
		channel.BasicPublish(exchange: "direct_logs",
							 routingKey: severity,
							 basicProperties: null,
							 body: body);
							 
		Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
	}



	
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();