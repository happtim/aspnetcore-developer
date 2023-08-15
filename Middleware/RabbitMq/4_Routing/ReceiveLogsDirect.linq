<Query Kind="Statements">
  <NuGetReference Version="6.5.0">RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
</Query>

//https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html
// 上一个教程我们使用fanout的exchange，将消息广播到绑定上queue中。
// 我们希望根据message的严重程度过滤消息。比如可以将严重报警写入磁盘。

//clone 本文件的参数配置
//var args = new string[] {"error"}; 
var args = new string[] {"info", "warning", "error"};

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	channel.ExchangeDeclare(exchange: "direct_logs",type: "direct");
	var queueName = channel.QueueDeclare().QueueName;

	foreach (var severity in args)
	{
		// bing方法是exchange和queue建立关联关系
		// bing可以指定routingKey，为了和BasicPublish中的参数混淆，我们叫他binding key
		// binging key的意义在每个类型exchange中不同。
		//	fanout：忽略
		//  direct：发布message的routing key 完全匹配binding key （QueueBind的routingkey）。
		channel.QueueBind(queue: queueName,
						  exchange: "direct_logs",
						  routingKey: severity);
	}

	Console.WriteLine(" [*] Waiting for messages.");

	var consumer = new EventingBasicConsumer(channel);
	consumer.Received += (model, ea) =>
	{
		var body = ea.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		var routingKey = ea.RoutingKey;
		Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
	};
	
	channel.BasicConsume(queue: queueName,
						 autoAck: true,
						 consumer: consumer);

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}
