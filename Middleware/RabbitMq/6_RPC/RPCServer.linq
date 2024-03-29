<Query Kind="Statements">
  <NuGetReference Version="6.5.0">RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
</Query>



var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	channel.QueueDeclare(queue: "rpc_queue", durable: false,  exclusive: false, autoDelete: false, arguments: null);
	channel.BasicQos(0, 1, false);

	var consumer = new EventingBasicConsumer(channel);

	consumer.Received += (model, ea) =>
	{
		string response = null;

		var body = ea.Body.ToArray();
		var props = ea.BasicProperties;
		var replyProps = channel.CreateBasicProperties();
		replyProps.CorrelationId = props.CorrelationId;

		try
		{
			var message = Encoding.UTF8.GetString(body);
			int n = int.Parse(message);
			Console.WriteLine(" [.] fib({0}) {1}", message,DateTime.Now);
			response = fib(n).ToString();
		}
		catch (Exception e)
		{
			Console.WriteLine(" [.] " + e.Message);
			response = "";
		}
		finally
		{
			var responseBytes = Encoding.UTF8.GetBytes(response);
			channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
			channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
		}
	};
	
	channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
	Console.WriteLine(" [x] Awaiting RPC requests");

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}


static int fib(int n)
{
	if (n == 0 || n == 1)
	{
		return n;
	}

	return fib(n - 1) + fib(n - 2);
}