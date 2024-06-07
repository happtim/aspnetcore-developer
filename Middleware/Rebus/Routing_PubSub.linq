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

//await bus.Subscribe<MyMessage>();
// 如果 SubscribeStrore是中心的。那么直接将订阅的类型于transport的queueName 绑定。
// 如果 SubscribeStrore不是中心的。那么将通过Routing的规则配置，将queueName发送到订阅服务器。支持分布式的订阅。可以订阅不同服务器。

//await bus.Publish(eventMessage);
//从SubscribeStrore从查找消息topic配置的队列名称。

// 使用BuiltinHandlerActivator来注册消息处理程序
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new MyMessageHandler());

// 配置Rebus
var bus = Configure.With(activator)
	.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
	
	//s.StoreInMemory(new InMemorySubscriberStore()) 使用的 IsCentralized = true配置。 订阅的时候直接与队列绑定。
	.Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
	
	//StoreInMemory 使用的时候 IsCentralized = false配置。 需要从 TypeBased 中获取对列名。
	//.Subscriptions(s => s.StoreInMemory())
	//.Routing(r => r.TypeBased().Map<MyMessage>("my-queue"))
	
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
