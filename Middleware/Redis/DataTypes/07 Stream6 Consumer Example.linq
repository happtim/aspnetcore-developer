<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

var consumerName = "C1";
var groupName = "C1Group";
var key = "mystream";
var lastid = "0-0";
var check_backlog = true;

var groupInfo = db.StreamGroupInfo(key);
if (!groupInfo.Any(i => i.Name == groupName)) 
{
	db.StreamCreateConsumerGroup(key, groupName, "$", createStream: true);
}

while (true)
{
	//# Pick the ID based on the iteration: the first time we want to
	//# read our pending messages, in case we crashed and are recovering.
	//# Once we consumed our history, we can start getting new messages.
	string myid = "" ;
	if(check_backlog)
		myid = lastid;
	else
		myid = ">";
	
	var items = db.StreamReadGroup(key,groupName,consumerName,myid,10);

	//# If we receive an empty reply, it means we were consuming our history
	//# and that the history is now empty. Let's start to consume new messages.
	if (items.Count() == 0) 
	{
		check_backlog = false ;
		Thread.Sleep(1000);
	}
	
	foreach (var item in items)
	{
		item.Values.Select(s =>s.ToString() ).Dump("received message");
		db.StreamAcknowledge(key,groupName, item.Id);
		lastid = item.Id;
	}
	
	Thread.Sleep(1000);
}