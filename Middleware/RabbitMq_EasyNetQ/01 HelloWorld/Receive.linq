<Query Kind="Statements">
  <NuGetReference>EasyNetQ</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
</Query>


using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
	bus.PubSub.Subscribe<TextMessage>("", HandleTextMessage);
	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}

static void HandleTextMessage(TextMessage textMessage)
{
	Console.WriteLine(" [x] Received {0}", textMessage.Text);
}

[Queue("hello", ExchangeName = "")]
public class TextMessage
{
	public string Text { get; set; }
}
