<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <Namespace>Rebus.Activation</Namespace>
  <Namespace>Rebus.Config</Namespace>
  <Namespace>Rebus.Transport.InMem</Namespace>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// we have the container adapter in a variable here, but you should stash it 
// in a static field somewhere, and then dispose it when your app shuts down
using var activator = new BuiltinHandlerActivator();

activator.Register(() => new PrintDateTime());

Configure.With(activator)
	.Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "my-queue"))
	.Start();

var timer = new System.Timers.Timer();
timer.Elapsed += delegate { activator.Bus.SendLocal(DateTime.Now).Wait(); };
timer.Interval = 1000;
timer.Start();

Console.WriteLine("Press enter to quit");
Console.ReadLine();

public class PrintDateTime : IHandleMessages<DateTime>
{
	public async Task Handle(DateTime currentDateTime)
	{
		Console.WriteLine("The time is {0}", currentDateTime);
	}
}