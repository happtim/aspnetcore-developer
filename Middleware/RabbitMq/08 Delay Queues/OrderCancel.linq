<Query Kind="Statements">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
</Query>



//

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{

	channel.ExchangeDeclare(exchange: "exchange.dlx", type: ExchangeType.Direct);

	channel.ExchangeDeclare(exchange: "exchange.normal", type: ExchangeType.Fanout);
	//通过在channel.queueDeclare方法中设置x-message-ttl 设置队列的过期时间。
	Dictionary<string, object> dictionary = new Dictionary<string, object>();
	dictionary.Add("x-message-ttl", 10*1000);
	//通过在channel.queueDeclare方法中设置x-dead-letter-exchange参数来为这个队列添加DLX。
	dictionary.Add("x-dead-letter-exchange", "exchange.dlx");
	//也可以为这个DLX指定路由键，如果没有特殊指定，则使用原队列的路由键
	//dictionary.Add("x-dead-letter-routing-key","dlx-rouing-key");
	
	channel.QueueDeclare(queue: "queue.normal", arguments: dictionary);
	channel.QueueBind("queue.normal", "exchange.normal", "");

	channel.QueueDeclare(queue: "queue.dlx");
	channel.QueueBind("queue.dlx", "exchange.dlx", "");


	var consumer = new EventingBasicConsumer(channel);
	consumer.Received += (model, ea) =>
	{
		var body = ea.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		Console.WriteLine(" [x] {0}", message);
	};
	channel.BasicConsume(queue: "queue.dlx",
						 autoAck: true,
						 consumer: consumer);

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();

}

