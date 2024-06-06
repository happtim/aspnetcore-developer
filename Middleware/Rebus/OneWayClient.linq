<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
</Query>

//在One-Way Client 方式中。发送的客户端不会创建Worker。

// 创建一个内存传输的后端
var network = new InMemNetwork();

// 配置消息接收端
using var receiver = new BuiltinHandlerActivator();

//注册消息接受处理
receiver.Register(() => new MyMessageHandler());

Configure.With(receiver)
	.Transport(t => t.UseInMemoryTransport(network, "receiver"))
	.Start();

// 配置消息发送端 (One-Way)
var sender = Configure.With(new BuiltinHandlerActivator())
	.Transport(t => t.UseInMemoryTransportAsOneWayClient(network))
	.Routing(r => r.TypeBased().Map<MyMessage>("receiver"))
	.Start();

// 发送消息
var timer = new System.Timers.Timer();
timer.Elapsed += delegate { sender.Send(new MyMessage { Text = "Hello, In-Memory One-Way!" }); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Message sent. Press Enter to quit.");
Console.ReadLine();


public class MyMessage
{
	public string Text { get; set; }
}

public class MyMessageHandler : IHandleMessages<MyMessage>
{
	public async Task Handle(MyMessage message)
	{
		Console.WriteLine("Received message: " + message.Text);
	}
}