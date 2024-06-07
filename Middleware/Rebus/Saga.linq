<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <NuGetReference Version="7.3.1">Rebus.SqlServer</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>Rebus.Persistence.InMem</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\MyMessageHandler"

//配置 Rebus 将 sagas（在文献中称为过程管理器）持久化到数据库中相对简单。
//默认情况下，SQL Server saga 存储将会确保在配置中指定的两个表"Sagas", "SagaIndex"被创建。需要先创建数据库

// 使用BuiltinHandlerActivator来注册消息处理程序
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new MyMessageHandler());

var connectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=Rebus;Trusted_Connection=True;TrustServerCertificate=True";

// 配置Rebus
var bus = Configure.With(activator)
	.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
	.Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
	.Sagas(s => s.StoreInSqlServer(connectionString, "Sagas", "SagaIndex"))
	//.Routing(r => r.TypeBased().Map<MyMessage>("my-queue"))
	.Start();

// 订阅HelloMessage
await bus.Subscribe<MyMessage>();

// 发送消息
var timer = new System.Timers.Timer();
timer.Elapsed += delegate { bus.Publish(new MyMessage { Text = "Hello, Saga!" + DateTime.Now.ToString() }); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();
