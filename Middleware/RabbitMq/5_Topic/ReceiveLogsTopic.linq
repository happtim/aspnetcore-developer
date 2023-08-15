<Query Kind="Statements">
  <NuGetReference Version="6.5.0">RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
</Query>


// 在上一个教程中我们改进了订阅方式更具bingkey，但是还是有一些局限。他不能进行多条件路由。
// 比如需求是：不光要订阅日志的严重性，还有订阅日志的来源。
// 我们需要一个更复杂的exchange类型：topic

// 发送到topic exchange中的消息的routingkey需要一个是单词列表，至少0个.来分隔。
// 例如： “quick.orange.rabbit” 最多长达255字节。
// 发送消息到exchange中会根据routingkey匹配exchange绑定的queue。规则如下：
// *（星号）可以代替一个单词。
// #（井号）可以替换零个或多个单词。

//var args = new string[] {"lazy.*.*"};
var args = new string[] { "quick.orange.*" , };

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
	var queueName = channel.QueueDeclare().QueueName;

	foreach (var bindingKey in args)
	{
		channel.QueueBind(queue: queueName,  exchange: "topic_logs",  routingKey: bindingKey);
	}

	Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

	var consumer = new EventingBasicConsumer(channel);
	consumer.Received += (model, ea) =>
	{
		var body = ea.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		var routingKey = ea.RoutingKey;
		Console.WriteLine(" [x] Received '{0}':'{1}'",routingKey, message);
	};
	channel.BasicConsume(queue: queueName,autoAck: true, consumer: consumer);

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}