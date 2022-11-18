<Query Kind="Program">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

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

		props = channel.CreateBasicProperties();
		var correlationId = Guid.NewGuid().ToString();
		props.CorrelationId = correlationId;
		props.ReplyTo = replyQueueName;
		
		
		consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var response = Encoding.UTF8.GetString(body);
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