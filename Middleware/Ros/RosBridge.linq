<Query Kind="Statements">
  <NuGetReference>rosbridge-client-ros</NuGetReference>
  <Namespace>RosSharp.RosBridgeClient</Namespace>
  <Namespace>RosSharp.RosBridgeClient.MessageTypes.Rosapi</Namespace>
</Query>


string uri = "ws://127.0.0.1:9090";

RosSocket rosSocket  = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));

//查看topics
rosSocket.CallService<TopicsRequest ,TopicsResponse >("/rosapi/topics",TopicsResponseHandler, new TopicsRequest());

//查看service
rosSocket.CallService<ServicesRequest,ServicesResponse>("/rosapi/services", ServicesResponseHandler,  new ServicesRequest());

//查看nodes
rosSocket.CallService<NodesRequest,NodesResponse>("/rosapi/nodes", NodesResponseHandler,  new NodesRequest());

//查看param names
rosSocket.CallService<GetParamNamesRequest,GetParamNamesResponse>("/rosapi/get_param_names", GetParamNamesResponseHandler,  new GetParamNamesRequest());

//查看单个param
rosSocket.CallService<GetParamRequest, GetParamResponse>("/rosapi/get_param", GetParamResponseHandler, new GetParamRequest() { name = "/rosdistro"});

Console.ReadLine();
rosSocket.Close();


void TopicsResponseHandler(TopicsResponse message)
{
	message.topics.OrderBy(t => t).Dump("topics");
}

void ServicesResponseHandler(ServicesResponse message)
{
	message.services.OrderBy(s =>s ).Dump("services");
}

void NodesResponseHandler(NodesResponse message)
{
	message.nodes.OrderBy(n => n).Dump("nodes");
}

void GetParamNamesResponseHandler(GetParamNamesResponse message)
{
	message.names.OrderBy(n => n.Trim()).Dump("param names");
}

void GetParamResponseHandler(GetParamResponse message)
{
	message.Dump("param /rosdistro");
}


