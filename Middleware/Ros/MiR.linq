<Query Kind="Statements">
  <NuGetReference>rosbridge-client-ros</NuGetReference>
  <Namespace>RosSharp.RosBridgeClient</Namespace>
  <Namespace>RosSharp.RosBridgeClient.MessageTypes.Rosapi</Namespace>
</Query>


string uri = "ws://192.168.12.20:9090";

RosSocket rosSocket  = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri));

//查看topics
//rosSocket.CallService<TopicsRequest ,TopicsResponse >("/rosapi/topics",TopicsResponseHandler, new TopicsRequest());

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

//小车状态
//rosSocket.Subscribe<RobotStatus>("/robot_status", RobotStatusHandler);

//寄存器事件
//rosSocket.Subscribe<PLCRegisterEvent>("/data_events/registers",PLCRegisterEventHandler);

//MiR100 bms状态
//rosSocket.Subscribe<LightCtrlBmsStatus>("/LightCtrl/bms_data",LightCtrlBmsStatusHandler );

//MiR 其他型号bms状态
//rosSocket.Subscribe<BmsStatus>("/PB/bms_status",BmsStatusHandler );

//站点数据变更
//rosSocket.Subscribe<DataEventsPositions>("/data_events/positions",DataEventsPositionsHandler );

Console.ReadLine();

void DataEventsPositionsHandler(DataEventsPositions message)
{
	message.Dump();
}

void RobotStatusHandler(RobotStatus message)
{
	message.Dump();
}

void PLCRegisterEventHandler(PLCRegisterEvent message)
{
	message.Dump();
}

void LightCtrlBmsStatusHandler(LightCtrlBmsStatus message)
{
	message.Dump();	
}

void BmsStatusHandler(BmsStatus message)
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


public class PLCRegisterEvent : Message
{
	public const string RosMessageName = "mir_data_msgs/PLCRegisterEvent";

	public PLCRegister data { get; set; } //PLC寄存器数据

	public class PLCRegister
	{
		public int id { get; set; }
        public float value { get; set; }
		public string label { get; set; }
	}

}

/// <summary>
/// Mir100 Suport BmsStatus,the "/PB/bms_status" not return data
/// </summary>
public class LightCtrlBmsStatus : Message
{
	public const string RosMessageName = "mirMsgs/BMSData";
	public int temperature { get; set; } //温度1
	public int cycle_count { get; set; } //循环次数
	public int[] cell_voltage { get; set; } //电芯电压

}

public class BmsStatus : Message
{
	public const string RosMessageName = "mirMsgs/BMSData";
	//序列化数据需要都小写
	public int temperature { get; set; } //温度1
	public int cycle_count { get; set; } //循环次数
	public int[] cell_voltage { get; set; } //电芯电压
}

public class DataEventsPositions : Message
{
	public const string RosMessageName = "mir_data_msgs/PositionEvent";
	
    public Position data { get; set; }

	public class Position
	{
		public string guid { get; set; }
		public string name { get; set; }
		public double pos_x { get; set; }
		public double pos_y { get; set; }
		public double orientation { get; set; }
		public int type_id { get; set; }
		public string map_id { get; set; }
		public string parent_id { get; set; }
		public string created_by_id { get; set; }
	}
}
