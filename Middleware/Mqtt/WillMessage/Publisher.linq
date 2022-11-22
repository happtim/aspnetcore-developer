<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
  <Namespace>MQTTnet.Protocol</Namespace>
</Query>


//最后的遗嘱消息用于通知订阅者发布者意外关闭。
//每个主题都可以在代理上存储最后遗嘱和遗嘱消息。

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder()
		.WithTcpServer("127.0.0.1")
		.WithWillTopic("my/last/will")
		.WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
		.WithWillPayload("lastwill")
		.Build();

	await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

	var applicationMessage = new MqttApplicationMessageBuilder()
		.WithTopic("rooms/room22/sensors/temp")
		.WithPayload("19.5")
		.WithRetainFlag()
		.Build();

	await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

	//Send a clean disconnect to the server，如果没有这个标识，则任务是non clean disconnect.
	//await mqttClient.DisconnectAsync();
	
	mqttClient.Dispose();// Dispose will not send a DISCONNECT pattern first so the will message must be sent.

	Console.WriteLine("MQTT application message is published.");
}