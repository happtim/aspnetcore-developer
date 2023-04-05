<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/streams-tutorial/#getting-data-from-streams

//获取数据的三种方式
	//对于流，我们希望多个使用者看到附加到流的新消息（与许多进程可以看到添加到日志中的内容的方式相同）。
	//使用传统术语，我们希望流能够将消息扇出到多个客户端。
	
	//不是作为消息传递系统，而是作为时间序列存储。在这种情况下，也许附加新消息也很有用，
	//但另一种自然查询模式是按时间范围获取消息，或者使用游标迭代消息以增量方式检查所有历史记录。
	
	//如果我们从消费者的角度看到一个流，我们可能希望以另一种方式访问该流，即作为消息流，
	//可以分区到正在处理此类消息的多个消费者，以便消费者组只能看到到达单个流的消息子集。

//Redis 流通过不同的命令支持上述所有三种查询模式。
//接下来的部分将展示所有这些内容，从最简单、最直接使用的开始：范围查询。

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

var key = "mystream";

db.KeyDelete(key);

var messageId =
db.StreamAdd(key, new NameValueEntry[] {
	new NameValueEntry("sensor-id",1234 ),
	new NameValueEntry("temperature",19.8)
	},"1518951480106-0");

var messageId2 =
db.StreamAdd(key, new NameValueEntry[] {
	new NameValueEntry("sensor-id",9999 ),
	new NameValueEntry("temperature",18.2)
	},"1518951482479-0");

// XRANGE somestream - +    (- + 代表数据的最小值id和最大值id)
//如果minId maxId 输入如 默认使用 - +
db.StreamRange(key).SelectMany(s => s.Values).Select(s => s.ToString() ).Dump(); 

//查询使用两个Unix times 毫秒时间戳，我们可以查询到所有这个时间间隔内的数据。
db.StreamRange(key,"1518951480106","1518951480107").SelectMany(s => s.Values).Select(s => s.ToString()).Dump();

//分页查询
//如果查询一个小时的数据时，这样的数据就非常大，我们可以使用Count参数，先获取前n个。
//然后再使用最后一个id继续查询。
db.KeyDelete(key);

//创建10个数据
for(int i = 1 ; i<= 10 ; i ++ )
{
	db.StreamAdd(key, new NameValueEntry[] {
		new NameValueEntry("foo", "value_"+ i ),
		});
		
	Thread.Sleep(2);
}
//是个查询前两个数据
var values =
db.StreamRange(key,count:2);

//查询前两个数据
values.SelectMany(s => s.Values).Select(s => s.ToString()).Dump("query first 2 item");

//查询3，4个数据
db.StreamRange(key, "("+values.Last().Id.ToString(), count: 2).SelectMany(s => s.Values).Select(s => s.ToString()).Dump("query next 2 item");

//倒叙查询
db.StreamRange(key,messageOrder: Order.Descending,count:2).SelectMany(s => s.Values).Select(s => s.ToString()).Dump("query last 2 item");
