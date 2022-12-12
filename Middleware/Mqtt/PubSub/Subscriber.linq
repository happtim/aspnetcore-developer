<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//客户端可以订阅单个或多个主题。
//订阅多个主题时，可以使用两个通配符。它们是：

//# (井号) – 多级通配符
//+ (加号) - 单级通配符

//通配符只能用于表示一个级别或多个级别，即 /house/#，而不是作为名称的一部分来表示多个字符，例如 hou#无效

// house/# 匹配
//house/room1/alarm
//house/garage/main-light
//house/main-door
//house/
//house

//house/+/main-light 匹配
//house/room1/main-light
//house/room2/main-light
//house/garage/main-light


//何时创建主题
//有人订阅了主题
//有人向主题发布消息，并将保留的消息设置为 True。

//何时从Broker删除主题
//当订阅该代理的最后一个客户端断开连接时，且clean session 为 true。
//当客户端连接时，clean session 设置为 True。

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

	await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

	var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
		.WithTopicFilter(
			f =>
			{
				//匹配多个topic的时候 如下写法会覆盖。实际订阅使用room2的topic
				f.WithTopic("rooms/room1/sensors/temp");
				f.WithTopic("rooms/room2/sensors/temp");
			})
		.WithTopicFilter(f =>
			{
				f.WithTopic("rooms/room3/sensors/temp");
			})
		.Build();


	// Setup message handling before connecting so that queued messages
	// are also handled properly. When there is no event handler attached all
	// received messages get lost.
	mqttClient.ApplicationMessageReceivedAsync += e => 
	{
		Console.WriteLine(e.ApplicationMessage.Topic + ": Received application message. " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload) );
		//e.Dump();
		return Task.CompletedTask;
	};

	var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

	Console.WriteLine("MQTT client subscribed to topic.");

	// The response contains additional data sent by the server after subscribing.
	//response.Dump();

	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}