<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
</Query>

/*
* This sample pushes a simple application message including a topic and a payload.
*
* Always use builders where they exist. Builders (in this project) are designed to be
* backward compatible. Creating an _MqttApplicationMessage_ via its constructor is also
* supported but the class might change often in future releases where the builder does not
* or at least provides backward compatibility where possible.
*/

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder()
		.WithTcpServer("127.0.0.1")
		.Build();

	await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

	//MQTT 支持 3 QOS levels 0,1,2.
	//QOS -0 – 默认值，不保证消息传递。
	//QOS - 1 – 保证消息传递，但可能会获得重复项。
	//QOS - 2 - 保证消息传递没有重复。
	//如果要尝试确保订阅者收到消息，即使他们可能不在线，则需要以 1 或 2 的服务质量发布。
	var applicationMessage = new MqttApplicationMessageBuilder()
		.WithTopic("rooms/room1/sensors/temp")
		.WithPayload("19.5")
		.WithQualityOfServiceLevel( MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
		.Build();

	await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

	await mqttClient.DisconnectAsync();

	Console.WriteLine("MQTT application message is published.");
}