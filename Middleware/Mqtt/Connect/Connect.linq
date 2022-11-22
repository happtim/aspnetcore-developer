<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
</Query>

/*
	   * This sample creates a simple MQTT client and connects to an invalid broker using a timeout.
	   * 
	   * This is a modified version of the sample _Connect_Client_! See other sample for more details.
	   */

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

	try
	{
		using (var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
		{
			// This will throw an exception if the server is not available.
			// The result from this message returns additional data which was sent 
			// from the server. Please refer to the MQTT protocol specification for details.
			var response = await mqttClient.ConnectAsync(mqttClientOptions, timeoutToken.Token);
			
			Console.WriteLine("The MQTT client is connected.");

			response.Dump();
			
			// Send a clean disconnect to the server by calling _DisconnectAsync_. Without this the TCP connection
			// gets dropped and the server will handle this as a non clean disconnect (see MQTT spec for details).
			var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();
			await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
		}
	}
	catch (OperationCanceledException)
	{
		Console.WriteLine("Timeout while connecting.");
	}
}