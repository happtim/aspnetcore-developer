<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <NuGetReference>MQTTnet.Extensions.Rpc</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
  <Namespace>MQTTnet.Extensions.Rpc</Namespace>
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

	using (var mqttRpcClient = mqttFactory.CreateMqttRpcClient(mqttClient))
	{
		// Access to a fully featured application message is not supported for RPC calls!
		// The method will throw an exception when the response was not received in time.
		var response =  await mqttRpcClient.ExecuteAsync(TimeSpan.FromSeconds(2), "ping", "123", MqttQualityOfServiceLevel.AtMostOnce);
		
		Encoding.UTF8.GetString(response).Dump();
	}

	Console.WriteLine("The RPC call was successful.");
}