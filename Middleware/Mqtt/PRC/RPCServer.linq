<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
  <Namespace>MQTTnet.Protocol</Namespace>
</Query>

var mqttFactory = new MqttFactory();

// The RPC client is an addon for the existing client. So we need a regular client
// which is wrapped later.

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder()
		.WithTcpServer("127.0.0.1")
		.Build();

	await mqttClient.ConnectAsync(mqttClientOptions);
	
	await mqttClient.SubscribeAsync("MQTTnet.RPC/+/ping", MqttQualityOfServiceLevel.AtMostOnce);

	mqttClient.ApplicationMessageReceivedAsync += async e => 
	{
		await mqttClient.PublishStringAsync(e.ApplicationMessage.Topic + "/response", "pong");
	};

	Console.WriteLine(" [x] Awaiting RPC requests");
	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();

}