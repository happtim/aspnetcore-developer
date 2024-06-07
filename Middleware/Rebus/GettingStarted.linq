<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#load ".\MyMessageHandler"

// we have the container adapter in a variable here, but you should stash it 
// in a static field somewhere, and then dispose it when your app shuts down
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new MyMessageHandler());

Configure.With(activator)
	.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
	.Start();

var timer = new System.Timers.Timer();
//默认Transport的地址发送消息
timer.Elapsed += delegate { activator.Bus.SendLocal(new MyMessage { Text = "Hello Rebus!" + DateTime.Now.ToString()}).Wait(); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Press enter to quit");
Console.ReadLine();

