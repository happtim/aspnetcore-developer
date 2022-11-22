<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <NuGetReference>MQTTnet.Extensions.ManagedClient</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
  <Namespace>MQTTnet.Extensions.ManagedClient</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

/*
* This sample creates a simple managed MQTT client and connects to a public broker.
*
* The managed client extends the existing _MqttClient_. It adds the following features.
* - Reconnecting when connection is lost.
* - Storing pending messages in an internal queue so that an enqueue is possible while the client remains not connected.
*/

var mqttFactory = new MqttFactory();

using (var managedMqttClient = mqttFactory.CreateManagedMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder()
		.WithTcpServer("127.0.0.1")
		.Build();

	var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
		.WithClientOptions(mqttClientOptions)
		.Build();

	await managedMqttClient.StartAsync(managedMqttClientOptions);

	managedMqttClient.ApplicationMessageReceivedAsync += e =>
	{
		Console.WriteLine(e.ApplicationMessage.Topic + ": Received application message. " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
		return Task.CompletedTask;
	};
	
	await managedMqttClient.SubscribeAsync("rooms/room1/sensors/temp");

	Console.WriteLine("The managed MQTT client is connected.");
	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}