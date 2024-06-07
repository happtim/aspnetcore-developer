<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <NuGetReference Version="7.3.1">Rebus.SqlServer</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>Rebus.Persistence.InMem</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
  <Namespace>Rebus.Timeouts</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\MyMessageHandler"

//当传输无法本地延迟消息传递时，Rebus 需要安装某种超时管理器。如下所示：
//Timeouts(t => t.StoreInSqlServer(connectionString, "Timeouts"))

// 使用BuiltinHandlerActivator来注册消息处理程序
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new MyMessageHandler());

var connectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=Rebus;Trusted_Connection=True;TrustServerCertificate=True";

// 配置Rebus
var bus = Configure.With(activator)
	.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
	.Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
	.Timeouts(t => t.StoreInSqlServer(connectionString, "Timeouts"))
	.Routing(r => r.TypeBased().Map<MyMessage>("my-queue"))
	.Start();

// 订阅HelloMessage
await bus.Subscribe<MyMessage>();

// 发送消息
var timer = new System.Timers.Timer();
timer.Elapsed += delegate { bus.Defer(TimeSpan.FromSeconds(5),new MyMessage { Text = "Hello, Defer!" + DateTime.Now.ToString() }); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();