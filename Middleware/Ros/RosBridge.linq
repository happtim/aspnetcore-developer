<Query Kind="Statements">
  <NuGetReference>rosbridge-client-ros</NuGetReference>
  <Namespace>RosSharp.RosBridgeClient</Namespace>
  <Namespace>RosSharp.RosBridgeClient.MessageTypes.Rosapi</Namespace>
</Query>


string uri = "ws://47.116.175.177:9090";

RosSocket rosSocket  = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));

rosSocket.CallService<TopicsRequest ,TopicsResponse >("/rosapi/topics",TopicsResponseHandler, new TopicsRequest());

rosSocket.Subscribe<RobotStatus>("/robot_status", SubscriptionHandler);

Console.ReadLine();


void SubscriptionHandler(RobotStatus message)
{
	message.Dump();
}

void TopicsResponseHandler(TopicsResponse message)
{
	message.Dump();
}

public class RobotStatus : Message
{
	public const string RosMessageName = "mirMsgs/RobotStatus";
	
	public bool joystick_low_speed_mode_enabled {get;set;}
}

