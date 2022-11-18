<Query Kind="Statements">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
</Query>


//日志产生者，发送的消息会经过一个exchange，投递到连接上的queue中。
//exchange的类型是：fanout。也就是将日志广播到所有连接队列上的消费者。

//Rabbitmq中消息传递模型的核心思想是Producer从从不直接将消息发送给Queue。实际上Producer并不知道消息是否会传递给那个消息队列。
//Producer只能将消息发送给Exchange，Exchange一边接受Producer消息，另一边推送到消息队列中。

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	//发送消息之前确保exchange存在，否则消息将会被丢弃。
	//Fanout类型：将收到消息广播到他所连接的Queue中。
	channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

	var message = "info:HelloWorld;";
	var body = Encoding.UTF8.GetBytes(message);
	 //在之前案例中我们并没有关注exchage,如果该值为空""，则使用默认exchange，类型（direct）。按照routingKey和队列关联。如果没有routingKey没有的话，那么该消息就丢弃了。
	 //该案例中我们将消息发送到logs exchange，如果该exchange没有bind对应队列。那么消息将会被丢弃。
	channel.BasicPublish(exchange: "logs",
						 routingKey: "",
						 basicProperties: null,
						 body: body);
	Console.WriteLine(" [x] Sent {0}", message);
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();