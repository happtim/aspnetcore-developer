<Query Kind="Program">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

// 本节提供了一个rpc的案例。 客户端调用远端的函数并等待返回结果。

//我们的 RPC 将像这样工作：
//
//当客户端启动时，它会创建一个anonymous exclusive  回调队列。
//对于 RPC 请求，客户端发送具有两个属性的消息：ReplyTo（设置为回调队列）和CorrelationId， 设置为每个请求的唯一值。
//请求将发送到rpc_queue队列。
//RPC worker （又名：server）正在等待该队列上的请求。 当请求出现时，它会完成作业并发送一条消息，其中包含 结果返回到客户端，使用ReplyTo属性中的队列。
//客户端等待回调队列中的数据。当消息 出现时，它将检查CorrelationId 属性。如果匹配 请求中的值，它将响应返回到 应用。

void Main()
{
	var rpcClient = new RpcClient();
	
	Console.WriteLine(" [x] Requesting fib(43)");
	var response = rpcClient.Call("43");
	
	Console.WriteLine(" [.] Got '{0}'", response);
	rpcClient.Close();

}

public class RpcClient
{
	private readonly IConnection connection;
	private readonly IModel channel;
	private readonly string replyQueueName;
	private readonly EventingBasicConsumer consumer;
	private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
	private readonly IBasicProperties props;

	public RpcClient()
	{
		var factory = new ConnectionFactory() { HostName = "localhost" };

		connection = factory.CreateConnection();
		channel = connection.CreateModel();
		replyQueueName = channel.QueueDeclare().QueueName;

		//The AMQP 0-9-1 protocol 定义了消息的14属性，大多数很少使用。以下列出了比较常用：
		// Persistent： 持久化消息。
		// DeliveryMode：Persistent做了同样的事情。
		// ContentType：用于描述编码的 MIME 类型。 常用的JSON编码：application/json
		// ReplyTo：通常用于命名回调队列。
		// CorrelationId：用于将 RPC 响应与请求相关联。
		props = channel.CreateBasicProperties();
		var correlationId = Guid.NewGuid().ToString();
		props.CorrelationId = correlationId;
		props.ReplyTo = replyQueueName;
		
		
		consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var response = Encoding.UTF8.GetString(body);
			//收到回调的队列中的CorrelationId要与请求一致。不一致则丢弃。
			//为什么不是异常？因为系统可能在发送ack之前挂了。那么当系统重新连接上来的时候将再次处理请求。
			if (ea.BasicProperties.CorrelationId == correlationId)
			{
				respQueue.Add(response);
			}
		};

		channel.BasicConsume(consumer: consumer, queue: replyQueueName,	autoAck: true);
	}

	public string Call(string message)
	{
		var messageBytes = Encoding.UTF8.GetBytes(message);
		channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: messageBytes);
		return respQueue.Take();
	}

	public void Close()
	{
		connection.Close();
	}
}