<Query Kind="Statements">
  <NuGetReference Version="6.0.1">Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <NuGetReference Version="7.0.0">Rebus.ServiceProvider</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>Rebus.Routing.TypeBased</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Rebus.Persistence.InMem</Namespace>
  <Namespace>Rebus.Bus</Namespace>
</Query>

//RedBus 支持容器。
//内容容器 BuiltinHandlerActivator()
//注册处理程序 activator.Register(() => new SomeHandler(pass, dependencies, into, ctor));

//Rebus.ServiceProvider 提供的 Microsoft.Extensions.DependencyInjection 注册服务。
//注册redbus services.AddRebus()
//注册处理程序 AddRebusHandler

// 创建服务集合
var services = new ServiceCollection();

// 配置 Rebus 传输和订阅存储
services.AddRebus(configure =>
	configure
		.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
		.Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
);

// 注册消息处理器
services.AddRebusHandler<MyMessageHandler>();
//services.AddTransient<IHandleMessages<MyMessage>, MyMessageHandler>();

// 构建服务提供者
var provider = services.BuildServiceProvider();

// 获取总线实例
var bus = provider.GetRequiredService<IBus>();

// 订阅消息类型
await bus.Subscribe(typeof(MyMessage));

// 启动总线Worker
var starter = provider.GetRequiredService<IBusStarter>();

starter.Start();

while (true)
{
	await bus.Publish(new MyMessage { Text = "Hello, Rebus! " + DateTime.Now.ToString() });
	await Task.Delay(1000);
}


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