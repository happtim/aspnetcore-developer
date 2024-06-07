<Query Kind="Statements">
  <NuGetReference Version="6.0.0">Microsoft.Extensions.Logging.Console</NuGetReference>
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <NuGetReference Version="4.0.0">Rebus.Microsoft.Extensions.Logging</NuGetReference>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>Rebus.Persistence.InMem</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\MyMessageHandler"
//您很可能已经有自己喜欢的记录方式，可能是使用 Serilog、NLog 或其他.NET 日志库。
//然后，您只需要配置 Rebus 以相同的方式传输其内部日志，这样您就可以在输出中交错地查看 Rebus 的工作和您的工作，从而能够理解应用程序中发生的一切。

// 使用BuiltinHandlerActivator来注册消息处理程序
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new MyMessageHandler());

var loggerFactory = LoggerFactory.Create(builder => 
{ 
	builder.AddConsole(); 
	builder.SetMinimumLevel(LogLevel.Debug);
});

// 配置Rebus
var bus = Configure.With(activator)
	.Logging(l => l.MicrosoftExtensionsLogging(loggerFactory))
	.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
	.Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
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
