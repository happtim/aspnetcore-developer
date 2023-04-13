<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://github.com/redis/redis-doc/blob/master/docs/data-types/streams-tutorial.md

//当任务是使用来自不同客户端的相同流时，XRead已经提供了一种Fan out到 N 个客户端的方法，可能还使用副本以提供更多的读取可伸缩性
//在某些问题中，我们想要做的不是向许多客户端提供相同的消息流，而是向许多客户端提供来自同一流的不同消息子集。

//为了实现这一点，Redis使用了一个称为消费者组的概念。从实现的角度来看，了解 Redis 消费者组与 Kafka （TM）功能上是相似的.

//一个消费者组就像一个伪消费者，它从流中获取数据，实际上服务于多个消费者，提供一定的保证：
//1每条消息都提供给不同的使用者，因此不可能将同一消息传递给多个使用者。
//2使用者组内由名称标识使用者，名称是实现使用者的客户端必须选择的区分大小写的字符串。
//	这意味着即使在断开连接后，流使用者组也会保留所有状态，因为客户端将再次声明为同一使用者。
//	但是，这也意味着由客户端提供唯一标识符。
//3每个使用者组都具有从未使用的第一个 ID 的概念，因此，当使用者请求新消息时，它可以仅提供以前未传递的消息。
//4但是，使用消息需要使用特定命令进行显式确认。Redis 将确认解释为：此消息已正确处理，因此可以从使用者组中逐出。
//5使用者组跟踪当前挂起的所有消息，即传递到使用者组的某个使用者但尚未确认为已处理的邮件。借助此功能，在访问流的消息历史记录时，每个使用者将只能看到传递给它的消息。
//在某种程度上，消费者组可以想象成关于流的某种状态：
//+----------------------------------------+
//| consumer_group_name: mygroup           |
//| consumer_group_stream: somekey         |
//| last_delivered_id: 1292309234234-92    |
//|                                        |
//| consumers:                             |
//|    "consumer-1" with pending messages  |
//|       1292309234234-4                  |
//|       1292309234232-8                  |
//|    "consumer-42" with pending messages |
//|       ... (and so forth)               |
//+----------------------------------------+


//这是理解是否要使用使用者组的方法：
	//如果您有一个流和多个客户端，并且希望所有客户端都获取所有消息，则不需要使用者组。
	//如果您有一个流和多个客户端，并且您希望跨客户端对流进行分区或分片，以便每个客户端都将获得到达流的消息子集，则需要一个使用者组。

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();


var key = "mystream";
var group1 = "mygroup";

db.KeyDelete(key);

//position 指定 $ 为只获取最新的消息
//position 指定 0 为过去还未消费的所有消息
//createStream 当没有stream 会报错，改方法true自动创建
db.StreamCreateConsumerGroup(key,group1,"0",createStream:true);

//创建数据
db.StreamAdd(key,"message","apple");
db.StreamAdd(key,"message","orange");
db.StreamAdd(key,"message","strawberry");
db.StreamAdd(key,"message","apricot");
db.StreamAdd(key,"message","banana");

//假设 Alice 和 Bob 是两个消费者 去过去Stream 中的水果
//参数 > 代表 消费者想要给消息从来没有投递给其他消费者，也就是一个新消息。
//如果位置参数指定 0或者其他的有效值， 那么就会返回这个id值之后pending列表中的消息。

//当调用XReadGroup方法时，服务器就记录将一个消息送达消费者手里，这个消息就记录再服务器的Pending Entries List (PEL)
//这个列表中的代表的消息为已送达但是没有恢复收到的消息。
//如果客户端发送 XACK消息之后 该消息就会从PEL中删除。 你也可以通过XPending 命令查看。


//Alice 获取一个新的消息
var apple =  db.StreamReadGroup(key,group1,"Alice",">",1);

//apple.Dump();

apple[0].Values.Select(s => s.ToString() ).Dump("Alice get apple");

//Alice 获取PendingList中数据
var apple2 =  db.StreamReadGroup(key,group1,"Alice","0",1);
apple2[0].Values.Select(s => s.ToString() ).Dump("Alice get apple from Pending Entries List");

//Alice 回复确认消息。
db.StreamAcknowledge(key,group1,apple2[0].Id);

//Alice 再获取PendingList中数据
apple2 =  db.StreamReadGroup(key,group1,"Alice","0-0",1);
apple2.Dump("peding entries list empty");

//Bob 读取两个消息
var some = db.StreamReadGroup(key,group1,"Bob",">",2);
some.SelectMany(s => s.Values).Select(s =>s.ToString()).Dump("Bob read Some");


//XReadGroup 需要注意几点
	//1.消费者在第一次被提及时会自动创建，无需显式创建。
	//2。即使您可以同时从多个key读取，但是要使其正常工作，您需要在每个流中创建具有相同名称的使用者组。
	//		这不是一个常见的需求，但值得一提的是，该功能在技术上是可用的。
	//3。XREADGROUP是一个 write 命令，因为即使它从流中读取，也会修改消费者组作为读取的副作用，因此只能在主实例上调用它。

