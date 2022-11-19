<Query Kind="Statements">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
</Query>


//DLX，全称为Dead-Letter-Exchange，可以称之为死信交换器，也有人称之为死信邮箱。
//当消息在一个队列中变成死信（dead message）之后，它能被重新被发送到另一个交换器中，这个交换器就是DLX，绑定DLX的队列就称之为死信队列。

//消息变成死信一般是由于以下几种情况：
//消息被拒绝（Basic.Reject / Basic.Nack），并且设置requeue参数为false；
//消息过期；
//队列达到最大长度。


//DLX也是一个正常的交换器，和一般的交换器没有区别，它能在任何的队列上被指定，实际上就是设置某个队列的属性。
//当这个队列中存在死信时，RabbitMQ就会自动地将这个消息重新发布到设置的DLX上去，进而被路由到另一个队列，即死信队列。

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	
	channel.ExchangeDeclare(exchange: "exchange.normal", type: ExchangeType.Fanout);

	var message = "生成订单成功，10s付款。否则取消。";
	var body = Encoding.UTF8.GetBytes(message);
	channel.BasicPublish(exchange: "exchange.normal",
						 routingKey: "",
						 basicProperties: null,
						 body: body);

	Console.WriteLine(" [x] Sent {0}", message);
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();