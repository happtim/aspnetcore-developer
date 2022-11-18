<Query Kind="Statements">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
</Query>


var args = new string[] { "quick.orange.fox", "quick.orange.rabbit", "lazy.orange.elephant" , "lazy.pink.rabbit" };

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	channel.ExchangeDeclare(exchange: "topic_logs",type: "topic");

	foreach (var routingKey in args)
	{
		var message = "Hello World!";

		var body = Encoding.UTF8.GetBytes(message);
		channel.BasicPublish(exchange: "topic_logs",	 routingKey: routingKey, basicProperties: null, body: body);

		Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
	}
}