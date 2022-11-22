<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
</Query>

/*
* This sample shows how to reconnect when the connection was dropped.
* This approach uses one of the events from the client.
* This approach has a risk of dead locks! Consider using the timer approach (see sample).
*/

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

	mqttClient.DisconnectedAsync += async e =>
	{
		if (e.ClientWasConnected)
		{
			Console.WriteLine("The MQTT client is disconnected.");
			//有风险重连失败。关闭 mosquitto 长时间之后就连接不上了。
			await mqttClient.ConnectAsync(mqttClient.Options);
		}
	};

	// 连接上订阅所有消息。
	mqttClient.ConnectedAsync += async e => 
	{
		Console.WriteLine("The MQTT client is connected.");
		await mqttClient.SubscribeAsync("#");
	};


	await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
	
	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}