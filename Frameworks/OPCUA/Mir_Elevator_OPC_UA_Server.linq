<Query Kind="Statements">
  <NuGetReference>OPCFoundation.NetStandard.Opc.Ua</NuGetReference>
  <Namespace>Opc.Ua.Server</Namespace>
  <Namespace>Opc.Ua</Namespace>
  <Namespace>Opc.Ua.Configuration</Namespace>
  <Namespace>Opc.Ua.Export</Namespace>
</Query>


ApplicationInstance application = new ApplicationInstance();
application.ApplicationName = "Elevator OPC UA Server";
application.ApplicationType = ApplicationType.Server;
application.ConfigSectionName = "ElevatorOpcUaServer";

try
{
	//// 加载应用程序配置
	//string directory = Path.GetDirectoryName(Util.CurrentQueryPath);
	//var configPath = Path.Combine(directory ,"Elevator.Config.xml");
	//ApplicationConfiguration config = await application.LoadApplicationConfiguration(configPath,false);

	ApplicationConfiguration config = new ApplicationConfiguration
	{
		ApplicationName = "Elevator OPC UA Server",
		ApplicationUri = "urn:localhost:ElevatorOpcUaServer",
		ProductUri = "http://yourcompany.com/ElevatorOpcUaServer",
		ApplicationType = ApplicationType.Server,
		
		SecurityConfiguration = new SecurityConfiguration
		{
			ApplicationCertificate = new CertificateIdentifier
			{
				StoreType = CertificateStoreType.X509Store,
				StorePath = "CurrentUser\\My",
				SubjectName = "CN=ElevatorOpcUaServer, C=US, S=Ohio, O=YourCompany"
			},
			TrustedIssuerCertificates = new CertificateTrustList
			{
				StoreType = "Directory",
				StorePath = "%CommonApplicationData%\\OPC Foundation\\CertificateStores\\UA Certificate Authorities"
			},
			TrustedPeerCertificates = new CertificateTrustList
			{
				StoreType = "Directory",
				StorePath = "%CommonApplicationData%\\OPC Foundation\\CertificateStores\\UA Applications"
			},
			RejectedCertificateStore = new CertificateTrustList
			{
				StoreType = "Directory",
				StorePath = "%CommonApplicationData%\\OPC Foundation\\CertificateStores\\RejectedCertificates"
			},
			AutoAcceptUntrustedCertificates = true,
			AddAppCertToTrustedStore = true,
		},
		TransportConfigurations = new TransportConfigurationCollection(),
		TransportQuotas = new TransportQuotas(),
		ServerConfiguration = new ServerConfiguration // 配置服务器选项
		{
			BaseAddresses = new StringCollection(new string[] 
			{
				"opc.tcp://0.0.0.0:4840"
			}),
			SecurityPolicies = new ServerSecurityPolicyCollection(new List<ServerSecurityPolicy>() 
			{
				new ServerSecurityPolicy
				{
					SecurityMode = MessageSecurityMode.None,
					SecurityPolicyUri = "http://opcfoundation.org/UA/SecurityPolicy#None",
				}
			})
		},
		TraceConfiguration = new TraceConfiguration
		{
			DeleteOnLoad = true,
			OutputFilePath = "%CommonApplicationData%\\OPC Foundation\\Logs\\ElevatorOpcUaServer.log.txt",
		}
	};
	
	application.ApplicationConfiguration = config;

	// 检查应用程序证书
	bool certOk = await application.CheckApplicationInstanceCertificate(false, 0);
	if (!certOk)
	{
		throw new Exception("Application instance certificate invalid!");
	}

	var server = new StandardServer();
	var nodeManagerFactory = new NodeManagerFactory();
	server.AddNodeManager(nodeManagerFactory);
	// 启动服务器
	await application.Start(server);

	Console.WriteLine("Elevator OPC UA Server is running. Press any key to exit.");
	Console.ReadLine();
	
	application.Stop();
}
catch (Exception ex)
{
	Console.WriteLine($"Exception: {ex.Message}");
	return;
}

public static partial class Namespaces
{
	/// <summary>
	/// The URI for the Alarms namespace (.NET code namespace is 'Elevator').
	/// </summary>
	public const string Elevators = "http://test.org/UA/Elevators/";
}

class NodeManagerFactory : INodeManagerFactory
{
	public ElevatorNodeManager? NodeManager { get; private set; }
	/// <inheritdoc/>
	public StringCollection NamespacesUris
	{
		get
		{
			var nameSpaces = new StringCollection {
					Namespaces.Elevators,
					Namespaces.Elevators + "Instance"
				};
			return nameSpaces;
		}
	}

	public INodeManager Create(IServerInternal server, ApplicationConfiguration configuration)
	{
		if (NodeManager != null)
			return NodeManager;

		NodeManager = new ElevatorNodeManager(server, configuration);
		return NodeManager;
	}
}

public class ElevatorNodeManager : CustomNodeManager2
{
	private const string NamespaceUri = "mir:elevator";
	private string directory = Path.GetDirectoryName(Util.CurrentQueryPath);
	private BaseDataVariableState _control;
	private BaseDataVariableState _requested_floor;
	private BaseDataVariableState _control_requested;

	public ElevatorNodeManager(IServerInternal server, ApplicationConfiguration configuration)
		: base(server, configuration, NamespaceUri)
	{
	}

	protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
	{

		NodeStateCollection predefinedNodes = new NodeStateCollection();
		var nodeSetPath = Path.Combine(directory ,"opc_ua-elevator-1.0.0.xml");
		
		using (FileStream fs = new FileStream(nodeSetPath, FileMode.Open))
		{
			var nodes = UANodeSet.Read(fs);
			nodes.Import(context,predefinedNodes);
		}

		var status_floor = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"status\".\"floor\"");
		status_floor.Value = 0;

		_requested_floor = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"status\".\"requested_floor\"");
		_requested_floor.Value = 0;

		_control_requested = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"status\".\"control_requested\"");
		_control_requested.Value = 0;

		_control = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"status\".\"control\"");
		_control.Value = 0;

		var door_front_full_open =(BaseDataVariableState)  predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"door\".\"front\".\"fully_open\"" );
		door_front_full_open.Value = false;
		
		var door_front_open_request = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"door\".\"front\".\"open_request\"");
		door_front_open_request.Value = false;

		var door_rear_full_open = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"door\".\"rear\".\"fully_open\"");
		door_rear_full_open.Value = false;

		var door_rear_open_request = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"door\".\"rear\".\"open_request\"");
		door_rear_open_request.Value = false;

		var fire_alarm = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"status\".\"fire_alarm\"");
		fire_alarm.Value = false;

		//elevator is available for fleet
		var is_available = (BaseDataVariableState)predefinedNodes.First(n => (string)n.NodeId.Identifier == "\"Elevator\".\"status\".\"is_available\"");
		is_available.Value = true;

		var version = (BaseDataVariableState)predefinedNodes.First(n => n.NodeId.IdType == IdType.String && (string)n.NodeId.Identifier == "\"Elevator\".\"version\"");
		version.Value = "1.0";
		
		return predefinedNodes;
	}

	protected override NodeState AddBehaviourToPredefinedNode(ISystemContext context, NodeState predefinedNode)
	{
		// add behaviour to our methods
		MethodState methodState = predefinedNode as MethodState;

		if (methodState != null)
		{
			if (methodState.DisplayName == "OpcMethodRequestControl") 
			{

				methodState.OnCallMethod = new GenericMethodCalledEventHandler(OnOpcMethodRequestControl);

				// 添加输入参数
				methodState.InputArguments = new PropertyState<Argument[]>(methodState)
				{
					NodeId = new NodeId(methodState.BrowseName.Name + "InArgs", NamespaceIndex),
					BrowseName = BrowseNames.InputArguments,
				};

				methodState.InputArguments.DisplayName = methodState.InputArguments.BrowseName.Name;
				methodState.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
				methodState.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
				methodState.InputArguments.DataType = DataTypeIds.Argument;
				methodState.InputArguments.ValueRank = ValueRanks.OneDimension;

				methodState.InputArguments.Value = new Argument[]
				{
					 new Argument
					 {
					 	Name = "Request Control",
					 	Description = "Request control of the server.",
						DataType = DataTypeIds.Boolean,
						ValueRank = ValueRanks.Scalar
					}
				};

				// 添加输出参数
				methodState.OutputArguments = new PropertyState<Argument[]>(methodState) 
				{
					NodeId = new NodeId(methodState.BrowseName.Name + "OutArgs", NamespaceIndex),
					BrowseName = BrowseNames.OutputArguments,
				};
				methodState.OutputArguments.DisplayName = methodState.OutputArguments.BrowseName.Name;
				methodState.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
				methodState.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
				methodState.OutputArguments.DataType = DataTypeIds.Argument;
				methodState.OutputArguments.ValueRank = ValueRanks.OneDimension;

				methodState.OutputArguments.Value = new Argument[]
				{
					new Argument{
						Name = "Control Granted",
						Description = "Indicates if control was granted.",
						DataType = DataTypeIds.Boolean,
						ValueRank = ValueRanks.Scalar,
					}
				};

			}

			if (methodState.DisplayName == "OpcMethodRequestDoor") 
			{

				methodState.OnCallMethod = new GenericMethodCalledEventHandler(OpcMethodRequestDoor);

				// 添加输入参数
				methodState.InputArguments = new PropertyState<Argument[]>(methodState)
				{
					NodeId = new NodeId(methodState.BrowseName.Name + "InArgs", NamespaceIndex),
					BrowseName = BrowseNames.InputArguments,
				};
				methodState.InputArguments.DisplayName = methodState.InputArguments.BrowseName.Name;
				methodState.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
				methodState.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
				methodState.InputArguments.DataType = DataTypeIds.Argument;
				methodState.InputArguments.ValueRank = ValueRanks.OneDimension;

				methodState.InputArguments.Value = new Argument[]
				{
					 new Argument
					 {
					 	Name = "Door front",
					 	Description = "Request door front.",
						DataType = DataTypeIds.Boolean,
						ValueRank = ValueRanks.Scalar
					},
					 new Argument
					 {
					 	Name = "Door rear",
					 	Description = "Request door rear.",
						DataType = DataTypeIds.Boolean,
						ValueRank = ValueRanks.Scalar
					},
					new Argument
					 {
					 	Name = "Door Open",
					 	Description = "Request door open.",
						DataType = DataTypeIds.Boolean,
						ValueRank = ValueRanks.Scalar
					}
				};

				// 添加输出参数
				methodState.OutputArguments = new PropertyState<Argument[]>(methodState)
				{
					NodeId = new NodeId(methodState.BrowseName.Name + "OutArgs", NamespaceIndex),
					BrowseName = BrowseNames.OutputArguments,
				};
				methodState.OutputArguments.DisplayName = methodState.OutputArguments.BrowseName.Name;
				methodState.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
				methodState.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
				methodState.OutputArguments.DataType = DataTypeIds.Argument;
				methodState.OutputArguments.ValueRank = ValueRanks.OneDimension;

				methodState.OutputArguments.Value = new Argument[]
				{
					new Argument{
						Name = "Request result",
						Description = "Request result.",
						DataType = DataTypeIds.Int16,
						ValueRank = ValueRanks.Scalar,
					}
				};
			}

			if (methodState.DisplayName == "OpcMethodRequestFloor")
			{

				methodState.OnCallMethod = new GenericMethodCalledEventHandler(OpcMethodRequestFloor);

				// 添加输入参数
				methodState.InputArguments = new PropertyState<Argument[]>(methodState)
				{
					NodeId = new NodeId(methodState.BrowseName.Name + "InArgs", NamespaceIndex),
					BrowseName = BrowseNames.InputArguments,
				};
				methodState.InputArguments.DisplayName = methodState.InputArguments.BrowseName.Name;
				methodState.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
				methodState.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
				methodState.InputArguments.DataType = DataTypeIds.Argument;
				methodState.InputArguments.ValueRank = ValueRanks.OneDimension;

				methodState.InputArguments.Value = new Argument[]
				{
					 new Argument
					 {
					 	Name = "Door front",
					 	Description = "Request door front.",
						DataType = DataTypeIds.Boolean,
						ValueRank = ValueRanks.Scalar
					},
					 new Argument
					 {
					 	Name = "Door rear",
					 	Description = "Request door rear.",
						DataType = DataTypeIds.Boolean,
						ValueRank = ValueRanks.Scalar
					},
					new Argument
					 {
					 	Name = "floor",
					 	Description = "Request floor.",
						DataType = DataTypeIds.Int16,
						ValueRank = ValueRanks.Scalar
					},
					new Argument
					 {
					 	Name = "end floor",
					 	Description = "go to after finishing.",
						DataType = DataTypeIds.Int16,
						ValueRank = ValueRanks.Scalar
					}
				};

				// 添加输出参数
				methodState.OutputArguments = new PropertyState<Argument[]>(methodState)
				{
					NodeId = new NodeId(methodState.BrowseName.Name + "OutArgs", NamespaceIndex),
					BrowseName = BrowseNames.OutputArguments,
				};
				methodState.OutputArguments.DisplayName = methodState.OutputArguments.BrowseName.Name;
				methodState.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
				methodState.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
				methodState.OutputArguments.DataType = DataTypeIds.Argument;
				methodState.OutputArguments.ValueRank = ValueRanks.OneDimension;

				methodState.OutputArguments.Value = new Argument[]
				{
					new Argument{
						Name = "Request result",
						Description = "Request result.",
						DataType = DataTypeIds.Int16,
						ValueRank = ValueRanks.Scalar,
					}
				};
			}

		}

		return predefinedNode;
	}

	private ServiceResult OnOpcMethodRequestControl(
	 ISystemContext context,
	 MethodState method,
	 IList<object> inputArguments,
	 IList<object> outputArguments)
	{
		try
		{
			// 检查输入参数
			if (inputArguments.Count != 1 || !(inputArguments[0] is bool))
			{
				return StatusCodes.BadInvalidArgument;
			}

			if ((bool)inputArguments[0]) 
			{
				_control.Value = 1;
				_control.ClearChangeMasks(context,false);
				_control_requested.Value = 1;
			}else
			{
				_control.Value = 0;
				_control.ClearChangeMasks(context,false);
				// 设置输出参数
				_requested_floor.Value = 0; //清除请求电梯
				_control_requested.Value = 0;
			}
			
			Console.WriteLine("OpcMethodRequestControl:"+inputArguments[0]);
			
			outputArguments[0] = (bool)inputArguments[0];

			return ServiceResult.Good;
		}
		catch (Exception ex)
		{
			return new ServiceResult(StatusCodes.Bad, ex.Message, ex);
		}
	}

	private ServiceResult OpcMethodRequestDoor(
	 ISystemContext context,
	 MethodState method,
	 IList<object> inputArguments,
	 IList<object> outputArguments)
	{
		try
		{
			// 检查输入参数
			if (inputArguments.Count != 3 || 
				!(inputArguments[0] is bool) || 
				!(inputArguments[1] is bool) ||
				!(inputArguments[2] is bool) )
			{
				return StatusCodes.BadInvalidArgument;
			}
			
			Console.WriteLine("OpcMethodRequestDoor:" + inputArguments[2]);

			// 设置输出参数
			outputArguments[0] = 1;

			return ServiceResult.Good;
		}
		catch (Exception ex)
		{
			return new ServiceResult(StatusCodes.Bad, ex.Message, ex);
		}
	}

	private ServiceResult OpcMethodRequestFloor(
	 ISystemContext context,
	 MethodState method,
	 IList<object> inputArguments,
	 IList<object> outputArguments)
		{
		try
		{
			// 检查输入参数
			if (inputArguments.Count != 4 ||
				!(inputArguments[0] is bool) ||
				!(inputArguments[1] is bool) ||
				!(inputArguments[2] is Int16) ||
				!(inputArguments[3] is Int16))
			{
				return StatusCodes.BadInvalidArgument;
			}
			
			Console.WriteLine("OpcMethodRequestFloor:" + inputArguments[2]);

			// 设置输出参数
			_requested_floor.Value = inputArguments[2]; //设置请求楼层
			outputArguments[0] = 1;

			return ServiceResult.Good;
		}
		catch (Exception ex)
		{
			return new ServiceResult(StatusCodes.Bad, ex.Message, ex);
		}
	}

}