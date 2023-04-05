<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://github.com/StackExchange/StackExchange.Redis/blob/main/docs/Streams.md

//从一个或多个流中读取数据，仅返回带有 ID 大于呼叫者报告的上次收到的 ID。 
//和XRange 比 可以读取多个stream。id筛选的id是大于。

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
ConnectionMultiplexer redis2 = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();
IDatabase db2  =redis2.GetDatabase();

var key = "mystream";

db.KeyDelete(key);

var messageId =
db.StreamAdd(key, new NameValueEntry[] {
	new NameValueEntry("sensor-id",1234 ),
	new NameValueEntry("temperature",19.8)
	}, "1518951480106-0");

var messageId2 =
db.StreamAdd(key, new NameValueEntry[] {
	new NameValueEntry("sensor-id",9999 ),
	new NameValueEntry("temperature",18.2)
	}, "1518951482479-0");
	

//从开始读取所有的数据.
db.StreamRead(key,"0-0").SelectMany(s => s.Values).Select(s => s.ToString()).Dump("read all messages");


var key2 = "foo_stream";
db.KeyDelete(key2);
//创建10个数据
for (int i = 1; i <= 10; i++)
{
	db.StreamAdd(key2, new NameValueEntry[] {
		new NameValueEntry("foo", "value_"+ i ),
		});

	Thread.Sleep(2);
}
xread(db,"client1");
xread(db2,"client2");

void xread(IDatabase database,string clientName)
{
	//从多个流中读取数据
	var streams = database.StreamRead(new StreamPosition[]
	{
	new StreamPosition(key, "0-0"),
	new StreamPosition(key2, "0-0")
	}, countPerStream: 2);

	streams.SelectMany(s => s.Entries).SelectMany(s => s.Values).Select(s => s.ToString()).Dump(clientName+" read from multiple streams");
	
	//对于Stream 属于自增的数据结构，删除是一个很奇怪的操作。我们可以使用XAdd命令来限制最大长度。

	//database.StreamDelete(key, streams[0].Entries.Select(s => s.Id).ToArray());
	//if (streams.Count() == 2)
	//{
	//	database.StreamDelete(key2, streams[1].Entries.Select(s => s.Id).ToArray());
	//}
	
	//再拿2个
	streams = database.StreamRead(new StreamPosition[]
	{
		new StreamPosition(key, streams[0].Entries.Last().Id.ToString()),
		new StreamPosition(key2, streams[1].Entries.Last().Id.ToString())
	}, countPerStream: 2);

	streams.SelectMany(s => s.Entries).SelectMany(s => s.Values).Select(s => s.ToString()).Dump(clientName +" read from multiple streams another 2");

}
