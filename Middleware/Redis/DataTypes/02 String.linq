<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/strings/

//Redis 字符串存储字节序列，包括文本、序列化对象和二进制数组。 
//因此，字符串是最基本的 Redis 数据类型。 
//它们通常用于缓存，但它们支持其他功能，这些功能也允许您实现计数器并执行按位运算。

//获取和设置字符串
	//SET 存储字符串值。
	//SETNX 仅当字符串值尚不存在时才会存储该值。对于实现锁很有用。
	//GET 检索字符串值。
	//MGET 在单个操作中检索多个字符串值。
//管理计数器
	//INCRBY以原子方式递增（并在传递负数时递减）存储在给定键上的计数器。
	//浮点计数器存在另一个命令：INCRBYFLOAT。
	
	//INCR 命令将字符串值解析为整数， 将其递增 1，最后将获取的值设置为新值。
	//INCR是原子的是什么意思？ 甚至多个客户针对 INCR 发布 INCR 同一键永远不会进入争用条件。
	//例如，它永远不会 碰巧客户端 1 读取“10”，客户端 2 同时读取“10”，两者都 递增到 11，并将新值设置为 11。
	//最终值将始终为 12和读取增量集操作在所有其他操作时执行 客户端不会同时执行命令。
//按位运算
	//若要对字符串执行按位运算，请参阅位图数据类型文档。

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();


string value = "abcdefg";
var key = "user:1";

//在 Redis 中存储并检索字符串：
db.StringSet(key, "salvatore");

//获取值
db.StringGet(key).ToString().Dump(key);

//当不存在key的时候才设置。SETNX
db.StringSet(key, "tim", when: When.NotExists);
db.StringGet(key).ToString().Dump(key);

//存储序列化的 JSON 字符串并将其设置为从现在起 100 秒过期：
var key2 = "ticket:27";
db.StringSet(key2,"{'username': 'priya', 'ticket_id': 321}",TimeSpan.FromSeconds(100));

//递增计数器：
var key3 = "views:page:2";
db.StringIncrement(key3).Dump();
db.StringIncrement(key3,10).Dump();


//设置新值，返回旧的值 6.2.0 命令废弃
db.StringGetSet(key, "tim").ToString().Dump("old value");

//设置新值，返回新值 6.2.0 推荐使用 set user:1 salvatore get
db.StringSetAndGet(key,"salvatore").ToString().Dump("new value");

