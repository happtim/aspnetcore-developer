<Query Kind="Statements">
  <NuGetReference>rosbridge-client-ros</NuGetReference>
  <Namespace>RosSharp.RosBridgeClient</Namespace>
  <Namespace>RosSharp.RosBridgeClient.MessageTypes.Rosapi</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


string uri = "ws://localhost:9090"; // ROSBridge服务器uri
var publisher = new RosPublisher(uri);
var subscribr = new RosSubscriber(uri);

while (true)
{
	publisher.Publish("Hello, ROS# from .NET Core!");
	await Task.Delay(1000); // 1秒发布一次
}

Console.ReadLine();

class RosSubscriber
{
	private RosSocket rosSocket;
	private string topic = "/chatter";
	
	public RosSubscriber(string uri)
	{
		rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));
		
		rosSocket.Subscribe<RosSharp.RosBridgeClient.MessageTypes.Std.String>(topic,MessageResponseHandler);
	}

	void MessageResponseHandler(RosSharp.RosBridgeClient.MessageTypes.Std.String message)
	{
		message.Dump();
	}
}

class RosPublisher
{
	private RosSocket rosSocket;
	private string topic = "/chatter";
	private string publicationId;

	public RosPublisher(string uri)
	{
		rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));

		publicationId = rosSocket.Advertise<RosSharp.RosBridgeClient.MessageTypes.Std.String>(topic);
	}

	public void Publish(string message)
	{
		var stringMessage = new RosSharp.RosBridgeClient.MessageTypes.Std.String
		{
			data = message
		};
		rosSocket.Publish(publicationId, stringMessage);
		//Console.WriteLine("Published: " + message);
	}

	public void Shutdown()
	{
		rosSocket.Close();
	}
}
