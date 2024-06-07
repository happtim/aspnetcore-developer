<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Rebus.Persistence.InMem</Namespace>
</Query>

#load ".\MyMessageHandler"

//没有原生发布/订阅支持的传输方式无法管理订阅和进行组播发送。
//订阅/取消订阅请求将被发送到特定事件类型的发布者，然后发布者可以将订阅保存到其本地订阅存储中。

// 使用BuiltinHandlerActivator来注册消息处理程序
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new MyMessageHandler());

// 配置Rebus
var bus = Configure.With(activator)
	.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
	.Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
	//.Routing(r => r.TypeBased().Map<MyMessage>("my-queue")) ？
	.Start();

// 订阅HelloMessage
await bus.Subscribe<MyMessage>();

// 发送消息
var timer = new System.Timers.Timer();
timer.Elapsed += delegate { bus.Publish(new MyMessage { Text = "Hello, ImMem!" + DateTime.Now.ToString() }); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();
