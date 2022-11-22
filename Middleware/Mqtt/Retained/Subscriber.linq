<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/*
 * This sample subscribes to a topic.
 */

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

	await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

	var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
		.WithTopicFilter(
			f =>
			{
				f.WithTopic("rooms/room20/sensors/temp");
			})
		.Build();


	// Setup message handling before connecting so that queued messages
	// are also handled properly. When there is no event handler attached all
	// received messages get lost.
	mqttClient.ApplicationMessageReceivedAsync += e =>
	{
		Console.WriteLine(e.ApplicationMessage.Topic + ": Received application message. " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
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