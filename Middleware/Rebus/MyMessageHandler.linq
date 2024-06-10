<Query Kind="Statements">
  <NuGetReference Version="6.6.5">Rebus</NuGetReference>
  <Namespace>Rebus.Handlers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


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

public class TMessageHandler<TMessage> : IHandleMessages<TMessage> 
	where TMessage : MyMessage
{
	public async Task Handle(TMessage message)
	{
		Console.WriteLine("Received message: " + message.Text);
	}
}