<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

//https://redis.io/docs/data-types/hashes/

//Redis 哈希是结构化为字段值对集合的记录类型。 

//基本命令
	//HSET 在哈希上设置一个或多个字段的值。
	//HGET 返回给定字段的值。
	//HMGET 返回一个或多个给定字段的值。
	//HINCRBY 将给定字段的值按提供的整数递增。

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

IDatabase db = redis.GetDatabase();

//将基本用户配置文件表示为哈希：
var key = "user:123";
db.HashSet(key, new HashEntry[] { 
	new HashEntry("username", "martina"),
	new HashEntry("firstName", "Martina"),
	new HashEntry("lastName", "Elisa"),
	} );

db.HashSet(key, new HashEntry[] {
	new HashEntry("country", "GB"),
});
	
db.HashGet(key,"username").ToString().Dump("username");

db.HashGetAll(key).Select(kv => kv.ToString() ).Dump("getall");


//存储设备 777 对服务器执行 ping 操作、发出请求或发送错误的次数的存储计数器：
var key2 ="device:777:stats";

db.KeyDelete(key2);
db.HashIncrement(key2,"pings",1);
db.HashIncrement(key2,"pings",1);
db.HashIncrement(key2,"pings",1);
db.HashIncrement(key2,"errors",1);
db.HashIncrement(key2,"requests",1);

//获取ping 次数
((int) db.HashGet(key2,"pings")).Dump("pings count");

//获取 errors  requests 次数
db.HashGet(key2, new RedisValue[] { "errors", "requests" }).Select( v => (int)v).Dump("errors and requests count");






