<Query Kind="Statements">
  <NuGetReference>rosbridge-client-ros</NuGetReference>
  <Namespace>RosSharp.RosBridgeClient</Namespace>
  <Namespace>RosSharp.RosBridgeClient.MessageTypes.Rosapi</Namespace>
</Query>


string uri = "ws://192.168.5.128:9090";

RosSocket rosSocket  = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));

//查看topics
rosSocket.CallService<TopicsRequest ,TopicsResponse >("/rosapi/topics",TopicsResponseHandler, new TopicsRequest());

//查看Topic类型
//rosSocket.CallService<TopicTypeRequest, TopicTypeResponse>("/rosapi/topic_type",TopicsTypeResponseHandler , new TopicTypeRequest("/data_events/registers"));

//查看service
//rosSocket.CallService<ServicesRequest,ServicesResponse>("/rosapi/services", ServicesResponseHandler,  new ServicesRequest());

//查看nodes
//rosSocket.CallService<NodesRequest,NodesResponse>("/rosapi/nodes", NodesResponseHandler,  new NodesRequest());

//查看param names
//rosSocket.CallService<GetParamNamesRequest,GetParamNamesResponse>("/rosapi/get_param_names", GetParamNamesResponseHandler,  new GetParamNamesRequest());

//查看单个param
//rosSocket.CallService<GetParamRequest, GetParamResponse>("/rosapi/get_param", GetParamResponseHandler, new GetParamRequest() { name = "/rosdistro"});


rosSocket.Subscribe<RobotStatus>("/robot_status", SubscriptionHandler);

Console.ReadLine();


void SubscriptionHandler(RobotStatus message)
{
	message.Dump();
}

void TopicsResponseHandler(TopicsResponse message)
{
	message.topics.OrderBy(t => t).Dump();
}

void TopicsTypeResponseHandler(TopicTypeResponse message)
{
	message.Dump();
}

void ServicesResponseHandler(ServicesResponse message)
{
	message.services.OrderBy(s =>s ).Dump();
}

void NodesResponseHandler(NodesResponse message)
{
	message.nodes.OrderBy(n => n).Dump();
}

void GetParamNamesResponseHandler(GetParamNamesResponse message)
{
	message.names.OrderBy(n => n.Trim()).Dump();
}

void GetParamResponseHandler(GetParamResponse message)
{
	message.Dump();
}


public class RobotStatus : Message
{
	public const string RosMessageName = "mirMsgs/RobotStatus";
	
	public bool joystick_low_speed_mode_enabled {get;set;}
}


