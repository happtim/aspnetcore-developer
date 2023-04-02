<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

ISubscriber sub = redis.GetSubscriber();

sub.Subscribe("messages", (channel, message) =>
{
	Console.WriteLine((string)message);
});

sub.Publish("messages", "hello");


