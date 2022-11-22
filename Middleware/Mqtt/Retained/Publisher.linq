<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
</Query>

//
//我们之前在讨论发布消息时提到了保留的消息。
//通常，如果发布者向主题发布消息，并且没有人订阅该主题，则代理会丢弃该消息。
//但是，发布者可以通过设置保留的消息标志来告诉代理保留该主题的最后一条消息。
//这可能非常有用，例如，如果传感器以较长的时间间隔发布其状态

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder()
		.WithTcpServer("127.0.0.1")
		.Build();

	await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

	var applicationMessage = new MqttApplicationMessageBuilder()
		.WithTopic("rooms/room20/sensors/temp")
		.WithPayload("19.5")
		.WithRetainFlag()
		.Build();

	await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

	await mqttClient.DisconnectAsync();

	Console.WriteLine("MQTT application message is published.");
}