<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/streams-tutorial/#streams-basics

//流是一种仅追加数据结构。 XADD的基本写入命令将新条目追加到指定的流。
//
//每个流条目由一个或多个字段值对组成，有点像 Redis Hash：

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

var key = "mystream";

db.KeyDelete(key);

var messageId =
db.StreamAdd(key, new NameValueEntry[] {
	new NameValueEntry("sensor-id",1234 ),
	new NameValueEntry("temperature",19.8)
	});
	
//返回实体id
messageId.ToString().Dump("auto-generated entry ID");

//长度
db.StreamLength(key).Dump("stream length");


//实体id
//实体id的格式为：<millisecondsTime>-<sequenceNumber>
//毫秒时间部分实际上是生成流 ID 的本地 Redis 节点中的本地时间，但是如果当前毫秒时间恰好小于上一个输入时间，
//则使用以前的进入时间，因此如果时钟向后跳转，单调递增的 ID 属性仍然成立。
//序列号用于在同一毫秒内创建的条目。由于序列号是 64 位宽，因此实际上对在同一毫秒内可以生成的条目数没有限制。

//如果由于某种原因，用户需要与时间无关但实际上与另一个外部系统 ID 关联的增量 ID
var key2 = "somestream";

db.KeyDelete(key2);

db.StreamAdd(key2, new NameValueEntry[] {
	new NameValueEntry("field","value" ),
	},
	"0-1").ToString().Dump("specified id 0-1");
	
db.StreamAdd(key2, new NameValueEntry[] {
	new NameValueEntry("foo","bar" ),
	},"0-2").ToString().Dump("specified id 0-2");

//请注意，在这种情况下，最小 ID 为 0-2，并且该命令将不接受等于或小于前一个 ID 的 ID：
//db.StreamAdd(key2, new NameValueEntry[] {
//	new NameValueEntry("foo","bar" ),
//	}, "0-1").ToString().Dump("specified id 0-1 again");

//从流中获取数据

