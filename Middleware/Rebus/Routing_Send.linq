<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
</Query>

#load ".\MyMessageHandler"

//“路由”是指 Rebus 根据您配置的某种逻辑来决定将消息发送到何处。
//可以指定 await bus.Advanced.Routing.Send("another.queue", someMessage); 指定endpoints。但是这样的配置hardcoding在大项目中不适用。
//因此，Rebus 具有额外的间接级别，通过在“端点映射”的形式中添加一些配置来帮助您路由消息。

//Send方法的处理：
//查看其端点映射以找到类型的所有者的队列名称，并发送给该队列。

// 创建一个内存传输的后端
var network = new InMemNetwork();

// 配置消息接收端
using var receiver = new BuiltinHandlerActivator();

//注册消息接受处理
receiver.Register(() => new MyMessageHandler());

Configure.With(receiver)
	.Transport(t => t.UseInMemoryTransport(network, "receiver"))
	.Start();
	
// 配置消息接收端2
using var receiver2 = new BuiltinHandlerActivator();
receiver2.Register(() => new MyMessageHandler());
Configure.With(receiver2)
	.Transport(t => t.UseInMemoryTransport(network, "receiver2"))
	.Start();


// 配置消息发送端 (One-Way)
var sender = Configure.With(new BuiltinHandlerActivator())
	.Transport(t => t.UseInMemoryTransportAsOneWayClient(network))
	.Routing(r => r.TypeBased().Map<MyMessage>("receiver")) //路由
	.Start();

// 发送消息
var timer = new System.Timers.Timer();
timer.Elapsed += delegate { sender.Send(new MyMessage { Text = "Hello, In-Memory One-Way!" }); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Message sent. Press Enter to quit.");
Console.ReadLine();
