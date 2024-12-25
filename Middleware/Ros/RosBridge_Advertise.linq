<Query Kind="Statements">
  <NuGetReference>rosbridge-client-ros</NuGetReference>
  <Namespace>RosSharp.RosBridgeClient</Namespace>
  <Namespace>RosSharp.RosBridgeClient.MessageTypes.Rosapi</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


/*

1. 创建Dockerfile

```
# 使用ROS Kinetic作为基础镜像
FROM ros:kinetic - ros - core

# 安装基本工具和依赖项
RUN apt-get update && apt-get install -y \
    python-rosdep \
    python-rosinstall \
    python-rosinstall-generator \
    python-wstool \
    build-essential \
    ros-kinetic-rosbridge-suite \
    && rm -rf /var/lib/apt/lists/*

# 初始化rosdep
RUN rosdep init

# 创建并初始化catkin工作区
RUN mkdir -p /root/catkin_ws/src
WORKDIR /root/catkin_ws
RUN /bin/bash -c "source /opt/ros/kinetic/setup.bash && catkin_make"

# 运行rosbridge服务器
CMD ["roslaunch", "rosbridge_server", "rosbridge_websocket.launch"]
```


如果在制作镜像的时候出现pull不下来。可以单独安装ros后运行。

`docker pull ros: kinetic - ros - core`

在包含Dockerfile的目录下，通过以下命令构建Docker镜像：

`docker build -t ros_kinetic_rosbridge.`

2.运行镜像

使用以下命令运行刚刚创建的Docker镜像：

`docker run -it --rm --name rosbridge_container -p 9090:9090 ros_kinetic_rosbridge`

*/

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
