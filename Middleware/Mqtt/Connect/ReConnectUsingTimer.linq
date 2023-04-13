<Query Kind="Statements">
  <NuGetReference>MQTTnet</NuGetReference>
  <Namespace>MQTTnet</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>MQTTnet.Client</Namespace>
</Query>

/*
	   * This sample shows how to reconnect when the connection was dropped.
	   * This approach uses a custom Task/Thread which will monitor the connection status.
	   * This is the recommended way but requires more custom code!
	   */

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
	var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1").Build();

	await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

	_ = Task.Run(
		async () =>
		{
					// User proper cancellation and no while(true).
			while (true)
			{
				try
				{
							// This code will also do the very first connect! So no call to _ConnectAsync_ is required in the first place.
					if (!await mqttClient.TryPingAsync())
					{
						//注意 重连之后需要重新订阅 消息 否则收不到消息了。
						await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

								// Subscribe to topics when session is clean etc.
						Console.WriteLine("The MQTT client is connected.");
					}
				}
				catch
				{
					Console.WriteLine("The MQTT client is disconnected.");
					// Handle the exception properly (logging etc.).
				}
				finally
				{
							// Check the connection state every 5 seconds and perform a reconnect if required.
					await Task.Delay(TimeSpan.FromSeconds(5));
				}
			}
		});
		
	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}