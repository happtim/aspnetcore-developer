<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <NuGetReference Version="7.4.6">Rebus.RabbitMq</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\MyMessageHandler"

//使用RabbitMQ 是那些原生支持发布/订阅的。因为发布/订阅“只需运行”，并且需要非常少的配置
//RabbitMQ发布/订阅的传输中会使用自己的订阅存储。无需额外的配置。

// 使用BuiltinHandlerActivator来注册消息处理程序
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new MyMessageHandler());

// 配置Rebus
var bus = Configure.With(activator)
	.Transport(t => t.UseRabbitMq("amqp://guest:guest@localhost:5672", "my-queue"))
	//.Routing(r => r.TypeBased().Map<MyMessage>("my-queue"))
	.Start();

// 订阅HelloMessage
await bus.Subscribe<MyMessage>();

// 发送消息
var timer = new System.Timers.Timer();
timer.Elapsed += delegate { bus.Publish(new MyMessage { Text = "Hello, RabbitMQ!" + DateTime.Now.ToString() }); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();
