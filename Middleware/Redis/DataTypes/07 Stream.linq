<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/streams/

//Redis 流是一种数据结构，其作用类似于仅追加日志。 您可以使用流实时记录和同时联合事件。 Redis 流用例的示例包括：
	//事件溯源（例如，跟踪用户操作、点击等）
	//传感器监控（例如，现场设备的读数）
	//通知（例如，将每个用户的通知记录存储在单独的流中）

//Redis 为每个流条目生成一个唯一的 ID。 您可以在以后使用这些 ID 检索其关联的条目，或读取和处理流中的所有后续条目。
//Redis 流支持多种trimming 策略（以防止流无限增长）和多个消费策略（ XREAD、XREADGROUP 和 XRANGE）。

//基本命令
	//XADD 向流添加新条目。
	//XREAD读取一个或多个条目，从给定位置开始并及时向前移动。
	//XRANGE 返回两个提供的条目 ID 之间的条目范围。
	//XLEN 返回流的长度。

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

var key = "temperatures:us-ny:10007";

db.KeyDelete(key);

db.StreamAdd(key, new NameValueEntry[] { 
	new NameValueEntry("temp_f",87.2),
	new NameValueEntry("pressure",29.69),
	new NameValueEntry("humidity",46),
	}).ToString().Dump("key1");

var messageID2 = 
db.StreamAdd(key, new NameValueEntry[] {
	new NameValueEntry("temp_f",83.1),
	new NameValueEntry("pressure",29.21),
	new NameValueEntry("humidity",46.5),
	});

db.StreamAdd(key, new NameValueEntry[] {
	new NameValueEntry("temp_f",81.9),
	new NameValueEntry("pressure",28.37),
	new NameValueEntry("humidity", 43.7),
	});
	
//读取从 ID 开始的前两个流条目
db.StreamRange(key,messageID2).SelectMany(s => s.Values).Select(s => s.ToString()).Dump("Read the first two stream entries starting at messageId2");

//从 0开始读取 300个流数据
db.StreamRead(key,StreamPosition.Beginning,100).SelectMany(s => s.Values).Select(s =>s.ToString() ).Dump("Read up to 100 new stream entries");

//获取stream个数
db.StreamLength(key).Dump("stream length");

