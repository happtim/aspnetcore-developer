<Query Kind="Statements">
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

// Rabbitmq 提供了发布消息的确认的功能，提供了消息的发布的可靠性。
// 当在Channel上开启了 publisher confirms。客户端发布的消息由 Broker 异步确认。

const int MESSAGE_COUNT = 50_000;

//开启Channel的Publisher confirms，它是RabbitMq在 AMQP 0.9.1 协议上的扩展，它默认是不开启的。
//同步等待确认：简单，但非常 吞吐量有限。
PublishMessagesIndividually();

//发布批处理
//简单、合理 吞吐量，但很难推理何时出现问题。
PublishMessagesInBatch();

//处理发布服务器异步确认
//最佳性能和资源利用率，在发生错误时具有良好的控制，但需要编写者正确实现。
HandlePublishConfirmsAsynchronously();

static IConnection CreateConnection()
{
	var factory = new ConnectionFactory { HostName = "localhost" };
	return factory.CreateConnection();
}

//这种方法的缺点就是大大减缓了发布的速度。
static void PublishMessagesIndividually()
{
	using var connection = CreateConnection();
	using var channel = connection.CreateModel();

	var queueName = channel.QueueDeclare(queue: "").QueueName;
	//开启 Publisher Confirms
	channel.ConfirmSelect();

	var timer = new Stopwatch();
	timer.Start();
	for (int i = 0; i < MESSAGE_COUNT; i++)
	{
		var body = Encoding.UTF8.GetBytes(i.ToString());
		channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
		//每发送一条数据我们使用下面方法进行确认，确认消息之后，该方法就立刻返回。
		//如果消息未在规定的时间内确认，该方法将会异常。
		// 异常的处理可以记录消息，或者重新发送消息。
		//此方法确实是异步的，当收到消息之后解除WaitForConfirmsOrDie对程序的阻塞。
		channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
	}
	timer.Stop();
	Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages individually in {timer.ElapsedMilliseconds:N0} ms");
}

static void PublishMessagesInBatch()
{
	using var connection = CreateConnection();
	using var channel = connection.CreateModel();

	// declare a server-named queue
	var queueName = channel.QueueDeclare(queue: "").QueueName;
	channel.ConfirmSelect();

	var batchSize = 100;
	var outstandingMessageCount = 0;
	var timer = new Stopwatch();
	timer.Start();
	for (int i = 0; i < MESSAGE_COUNT; i++)
	{
		var body = Encoding.UTF8.GetBytes(i.ToString());
		channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
		outstandingMessageCount++;

		//等待一批消息被确认可显著提高吞吐量 等待单个消息的确认。
		//如果发生故障，我们不知道到底出了什么问题。我们需要将一个批次缓存起来，以准备事故后重发。
		if (outstandingMessageCount == batchSize)
		{
			channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
			outstandingMessageCount = 0;
		}
	}

	if (outstandingMessageCount > 0)
		channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));

	timer.Stop();
	Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages in batch in {timer.ElapsedMilliseconds:N0} ms");
}



static void HandlePublishConfirmsAsynchronously()
{
	using var connection = CreateConnection();
	using var channel = connection.CreateModel();

	var queueName = channel.QueueDeclare(queue: "").QueueName;
	
	channel.ConfirmSelect();

	var outstandingConfirms = new ConcurrentDictionary<ulong, string>();

	void cleanOutstandingConfirms(ulong sequenceNumber, bool multiple)
	{
		if (multiple)
		{
			var confirmed = outstandingConfirms.Where(k => k.Key <= sequenceNumber);
			foreach (var entry in confirmed)
				outstandingConfirms.TryRemove(entry.Key, out _);
		}
		else
			outstandingConfirms.TryRemove(sequenceNumber, out _);
	}
	
	//使用异步回调的方式来实现异步通知的方式。
	//回调有个ea参数里面包含：
	//delivery tag:已确认的 sequence  或拒绝消息
	//multiple:这是一个布尔值。如果为 false，则只有一条消息被确认或拒绝，如果 True，则所有小于等于 sequence  的消息都将被确认/拒绝。减缓网络的压力。
	
	//消息确认回调。
	channel.BasicAcks += (sender, ea) => cleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
	
	//消息否定回调。
	channel.BasicNacks += (sender, ea) =>
	{
		outstandingConfirms.TryGetValue(ea.DeliveryTag, out string? body);
		Console.WriteLine($"Message with body {body} has been nack-ed. Sequence number: {ea.DeliveryTag}, multiple: {ea.Multiple}");
		cleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
	};

	var timer = new Stopwatch();
	timer.Start();
	for (int i = 0; i < MESSAGE_COUNT; i++)
	{
		var body = i.ToString();
		//通过NextPublishSeqNo 方法获取 sequence
		outstandingConfirms.TryAdd(channel.NextPublishSeqNo, i.ToString());
		channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: Encoding.UTF8.GetBytes(body));
	}

	if (!WaitUntil(60, () => outstandingConfirms.IsEmpty))
		throw new Exception("All messages could not be confirmed in 60 seconds");

	timer.Stop();
	Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages and handled confirm asynchronously {timer.ElapsedMilliseconds:N0} ms");
}

static bool WaitUntil(int numberOfSeconds, Func<bool> condition)
{
	int waited = 0;
	while (!condition() && waited < numberOfSeconds * 1000)
	{
		Thread.Sleep(100);
		waited += 100;
	}

	return condition();
}